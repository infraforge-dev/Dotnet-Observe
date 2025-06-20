using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Rendering;

/// <summary>
/// Renders log entries as a compact, single-line summary for fast scanning or grepping.
/// </summary>
public class CompactRenderer : ILogRenderer
{
    public void Render(LogEntry log)
    {
        Console.WriteLine(Format(log));
    }

    private static string Format(LogEntry log)
    {
        var timeStamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        var level = (log.Level ?? "INFO").ToUpperInvariant();
        var method = log.Context?.TryGetValue("Method", out var m) == true ? m?.ToString() : "METHOD";
        var path = log.Context?.TryGetValue("Path", out var p) == true ? p?.ToString() : "/";
        var status = log.Context?.TryGetValue("StatusCode", out var s) == true ? s?.ToString() : "N/A";
        var duration = log.Context?.TryGetValue("DurationMs", out var d) == true ? $"{d}ms" : "";

        var main = $"[{timeStamp}] [{level}] {method} {path} {status} ({duration})";

        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
            main += $" :: CorrelationId={log.CorrelationId}";

        if (log.Exception is not null)
            main += $" ⚠ {log.Exception.Message}";

        return main.Trim();
    }
}
