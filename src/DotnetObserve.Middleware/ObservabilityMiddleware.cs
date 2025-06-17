using System.Diagnostics;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using Microsoft.AspNetCore.Http;

namespace DotnetObserve.Middleware;

/// <summary>
/// Middleware that logs incoming requests and errors to the log store.
/// </summary>
public class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IStore<LogEntry> _logStore;

    public ObservabilityMiddleware(RequestDelegate next, IStore<LogEntry> logStore)
    {
        _next = next;
        _logStore = logStore;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var entry = new LogEntry
        {
            Source = "SampleApi",
            CorrelationId = Activity.Current?.TraceId.ToString()
        };

        try
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            entry.Level = "Info";
            entry.Message = $"{context.Request.Method} {context.Request.Path} returned {context.Response.StatusCode}";
            entry.Context = new Dictionary<string, object?>
            {
                ["DurationMs"] = stopwatch.ElapsedMilliseconds,
                ["StatusCode"] = context.Response.StatusCode,
                ["Method"] = context.Request.Method,
                ["Path"] = context.Request.Path.ToUriComponent()
            };
        }
        catch (Exception ex)
        {
            entry.Level = "Error";
            entry.Message = $"Exception thrown: {ex.Message}";
            entry.Exception = ex.ToString();
            throw;
        }
        finally
        {
            await _logStore.AppendAsync(entry);
        }
    }
}