using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Rendering;

/// <summary>
/// Renders log entries as simple plain text for console or file output.
/// </summary>
public class PlainTextRenderer : ILogRenderer
{
    public void Render(LogEntry log)
    {
        Console.WriteLine(Format(log));
    }

    private static string Format(LogEntry log)
    {
        var timeStamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        var level = log.Level?.ToUpperInvariant() ?? "INFO";
        var output = $"[{timeStamp}] [{level}] {log.Message}";

        if (!string.IsNullOrWhiteSpace(log.Source))
            output += $" (Source: {log.Source})";

        output += "\n";

        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
            output += $"  ↳ CorrelationId: {log.CorrelationId}\n";

        if (log.Context is { Count: > 0 })
        {
            var method = log.Context.TryGetValue("Method", out var m) ? m?.ToString() : "N/A";
            var path = log.Context.TryGetValue("Path", out var p) ? p?.ToString() : "N/A";
            var status = log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() : "N/A";
            var duration = log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() : "N/A";

            output += $"  ↳ Path: {path} | Method: {method} | Status: {status} | Duration: {duration}ms\n";

            foreach (var kvp in log.Context)
            {
                if (kvp.Key is "Path" or "Method" or "StatusCode" or "DurationMs")
                    continue;

                output += $"  ↳ {kvp.Key}: {kvp.Value}\n";
            }
        }

        if (log.Exception is not null)
        {
            output += $"  ⚠ Exception: {log.Exception.GetType().Name}: {log.Exception.Message}\n";
            output += $"    {log.Exception.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "[no stack trace]"}\n";
        }

        return output.TrimEnd();
    }
}
