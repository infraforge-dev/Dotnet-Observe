using DotnetObserve.Core.Models;
using Microsoft.AspNetCore.Http;
using DotnetObserve.Middleware;
using FluentAssertions;
using DotnetObserve.Core.Abstractions;

public class ObservabilityMiddlewareTests
{
    public class InMemoryObservabilityLogger : IObservabilityLogger
    {
        public List<LogEntry> Entries { get; } = new();

        public Task LogAsync(LogEntry entry)
        {
            Entries.Add(entry);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Middleware_Logs_Info_For_Successful_Request()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/test";

        var logger = new InMemoryObservabilityLogger();

        RequestDelegate next = async (ctx) =>
        {
            ctx.Response.StatusCode = 200;
            await ctx.Response.WriteAsync("OK");
        };

        var middleware = new ObservabilityMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var log = Assert.Single(logger.Entries);
        Assert.Equal("Info", log.Level);
        Assert.Equal("GET /test returned 200", log.Message);
        Assert.True(log.Context["StatusCode"] is int status && status == 200);
        Assert.Equal("/test", log.Context["Path"]);
    }

    [Fact]
    public async Task Middleware_logs_Warning_For_404_Response()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/not-found";

        var logger = new InMemoryObservabilityLogger();

        RequestDelegate next = async (ctx) =>
        {
            ctx.Response.StatusCode = 404;
            await ctx.Response.WriteAsync("Not Found");
        };

        var middleware = new ObservabilityMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var log = Assert.Single(logger.Entries);
        Assert.Equal("Warning", log.Level);
        Assert.Equal("GET /not-found returned 404", log.Message);
        Assert.Equal(404, Convert.ToInt32(log.Context!["StatusCode"]));
        Assert.Equal("/not-found", log.Context["Path"]);
    }

    [Fact]
    public async Task Middleware_Logs_Error_When_Exception_Is_Thrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/fail";

        var logger = new InMemoryObservabilityLogger();

        RequestDelegate next = (ctx) =>
        {
            throw new InvalidOperationException("Something broke");
        };

        var middleware = new ObservabilityMiddleware(next, logger);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        var log = Assert.Single(logger.Entries);
        Assert.Equal("Error", log.Level);
        Assert.Contains("Something broke", log.Message);
        Assert.Equal("/fail", log.Context!["Path"]);
        Assert.Equal("GET", log.Context["Method"]);

        log.Exception.Should().NotBeNull();
        log.Exception!.GetType().Name.Should().Be("InvalidOperationException");
        log.Exception!.Message.Should().Contain("Something broke");

        // If context still includes exception details (adjust as needed)
        Assert.NotNull(log.Context["ExceptionLocation"]);
    }
}
