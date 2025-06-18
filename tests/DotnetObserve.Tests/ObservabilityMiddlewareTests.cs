using DotnetObserve.Core.Storage;
using DotnetObserve.Core.Models;
using Microsoft.AspNetCore.Http;
using DotnetObserve.Middleware;

public class ObservabilityMiddlewareTests
{
    public class InMemoryStore<T> : IStore<T>
    {
        private readonly List<T> _items = new();

        public Task AppendAsync(T item)
        {
            _items.Add(item);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<T>> ReadAllAsync()
        {
            IReadOnlyCollection<T> result = _items.AsReadOnly();
            return Task.FromResult(result);
        }

        public IReadOnlyList<T> Items => _items;
    }

    [Fact]
    public async Task Middleware_Logs_Info_For_Successful_Request()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/test";

        var store = new InMemoryStore<LogEntry>();

        RequestDelegate next = async (ctx) =>
        {
            ctx.Response.StatusCode = 200;
            await ctx.Response.WriteAsync("OK");
        };

        var middleware = new ObservabilityMiddleware(next, store);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var log = Assert.Single(store.Items);
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

        var store = new InMemoryStore<LogEntry>();

        RequestDelegate next = async (ctx) =>
        {
            ctx.Response.StatusCode = 404;
            await ctx.Response.WriteAsync("Not Found");
        };

        var middleware = new ObservabilityMiddleware(next, store);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var log = Assert.Single(store.Items);
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

        var store = new InMemoryStore<LogEntry>();

        RequestDelegate next = (ctx) =>
        {
            throw new InvalidOperationException("Something broke");
        };

        var middleware = new ObservabilityMiddleware(next, store);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        var log = Assert.Single(store.Items);
        Assert.Equal("Error", log.Level);
        Assert.Contains("Something broke", log.Message);
        Assert.Equal("/fail", log.Context!["Path"]);
        Assert.Equal("GET", log.Context["Method"]);

        // Enriched exception details
        Assert.Equal("InvalidOperationException", log.Context["ExceptionType"]);
        Assert.Contains("Something broke", log.Context["ExceptionMessage"]?.ToString());
        Assert.NotNull(log.Context["StackTrace"]);
        Assert.NotNull(log.Context["ExceptionLocation"]);
    }
}
