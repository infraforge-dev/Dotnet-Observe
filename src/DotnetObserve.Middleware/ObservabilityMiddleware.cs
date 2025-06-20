using System.Diagnostics;
using System.Text.RegularExpressions;
using DotnetObserve.Core.Abstractions;
using DotnetObserve.Core.Constants;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using DotnetObserve.Core.Utils;
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
            Timestamp = DateTime.UtcNow,
            Source = "SampleApi",
            CorrelationId = Activity.Current?.TraceId.ToString()
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);

            int status = context.Response.StatusCode;
            entry.Level = LogLevelMapper.MapStatusToLogLevel(status);

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
                ["ExceptionLocation"] = FormatStackTracePath(ex.StackTrace)
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

    /// <summary>
    /// Extracts and cleans the first line of a stack trace, reducing it to a relative /src/... path.
    /// </summary>
    /// <param name="stackTrace">Raw exception stack trace.</param>
    /// <returns>Formatted location string like 'at /src/...:line ##', or first raw line if pattern not matched.</returns>
    private static string? FormatStackTracePath(string? stackTrace)
    {
        if (string.IsNullOrWhiteSpace(stackTrace)) return null;

        var firstLine = stackTrace.Split('\n').FirstOrDefault()?.Trim();
        if (firstLine is null) return null;

        var match = Regex.Match(firstLine, @"in (.*?)(:line \d+)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var fullPath = match.Groups[1].Value;
            var lineInfo = match.Groups[2].Value;

            var index = fullPath.IndexOf(@"\src\", StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                var relativePath = fullPath.Substring(index).Replace("\\", "/");
                return $"at {relativePath}{lineInfo}";
            }
        }

        return firstLine;
    }
}
