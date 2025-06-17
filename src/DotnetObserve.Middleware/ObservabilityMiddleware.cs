using System.Diagnostics;
using DotnetObserve.Core.Constants;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using Microsoft.AspNetCore.Http;

namespace DotnetObserve.Middleware;

/// <summary>
/// Middleware that automatically captures structured logs for each HTTP request, including timing,
/// status codes, and exceptions. Logs are written to an <see cref="IStore{T}"/> of <see cref="LogEntry"/>.
/// </summary>
public class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IStore<LogEntry> _logStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the ASP.NET Core pipeline.</param>
    /// <param name="logStore">The log store to which structured log entries will be written.</param>
    public ObservabilityMiddleware(RequestDelegate next, IStore<LogEntry> logStore)
    {
        _next = next;
        _logStore = logStore;
    }

    /// <summary>
    /// Executes the middleware logic for each incoming HTTP request.
    /// Captures timing, status codes, and exception metadata, and writes a structured log entry.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
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
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
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
