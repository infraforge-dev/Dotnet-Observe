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
}
