using System.Diagnostics;
using DotnetObserve.Core.Constants;
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

            int status = context.Response.StatusCode;

            entry.Level = status switch
            {
                >= 500 => LogLevels.Error,
                >= 400 => LogLevels.Warning,
                _ => LogLevels.Info
            };

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
            context.Response.StatusCode = 500;

            entry.Level = LogLevels.Error;
            entry.Message = $"Exception thrown: {ex.Message}";
            entry.Exception = ex.ToString();

            entry.Context = new Dictionary<string, object?>
            {
                ["Method"] = context.Request.Method,
                ["Path"] = context.Request.Path.ToUriComponent(),
                ["StatusCode"] = context.Response.StatusCode,
                ["ExceptionType"] = ex.GetType().Name,
                ["ExceptionLocation"] = ex.StackTrace?
                    .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim(),
                ["ExceptionMessage"] = ex.Message,
                ["StackTrace"] = ex.StackTrace
            };

            throw;
        }
        finally
        {
            await _logStore.AppendAsync(entry);
        }
    }
}