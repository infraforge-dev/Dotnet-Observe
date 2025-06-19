using System.Diagnostics;
using DotnetObserve.Core.Abstractions;
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
    private readonly IObservabilityLogger _logger;

    public ObservabilityMiddleware(RequestDelegate next, IObservabilityLogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var entry = new LogEntry
        {
            Source = "SampleApi",
            CorrelationId = Activity.Current?.TraceId.ToString()
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);

            int status = context.Response.StatusCode;
            entry.Level = status switch
            {
                >= 500 => LogLevels.Error,
                >= 400 => LogLevels.Warning,
                _ => LogLevels.Info
            };

            entry.Message = $"{context.Request.Method} {context.Request.Path} returned {context.Response.StatusCode}";
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;

            entry.Level = LogLevels.Error;
            entry.Message = $"Exception thrown: {ex.Message}";
            entry.Exception = ex;

            entry.Context = new Dictionary<string, object?>
            {
                ["ExceptionType"] = ex.GetType().Name,
                ["ExceptionLocation"] = ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim()
            };

            throw; // Re-throw to preserve exception behavior
        }
        finally
        {
            stopwatch.Stop();

            entry.Context ??= new Dictionary<string, object?>();
            entry.Context["DurationMs"] = stopwatch.ElapsedMilliseconds;
            entry.Context["StatusCode"] = context.Response.StatusCode;
            entry.Context["Method"] = context.Request.Method;
            entry.Context["Path"] = context.Request.Path.ToUriComponent();

            await _logger.LogAsync(entry);
        }
    }
}
