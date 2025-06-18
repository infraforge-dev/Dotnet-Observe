using DotnetObserve.Core.Constants;
using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Utils;

/// <summary>
/// Provides methods for formatting <see cref="LogEntry"/> objects into human-readable output for CLI display.
/// </summary>
public static class LogFormatter
{
    /// <summary>
    /// Formats a <see cref="LogEntry"/> using Spectre.Console markup with color-coded output and emoji indicators.
    /// </summary>
    /// <param name="log">The log entry to format.</param>
    /// <returns>A formatted string with ANSI markup for CLI display.</returns>
    public static string Format(LogEntry log)
    {
        var color = LogLevelStyles.GetColor(log.Level);
        var emoji = log.Level switch
        {
            "Error" => "❌",
            "Warn" or "Warning" => "⚠️",
            "Info" => "ℹ️",
            "Debug" => "🐛",
            "Trace" => "🔍",
            _ => ""
        };

        var levelMarkup = $"[{color}]{log.Level}[/]";
        var timeStamp = $"[grey]{log.Timestamp:yyyy-MM-dd HH:mm:ss}[/]";
        var message = log.Message;
        var source = !string.IsNullOrWhiteSpace(log.Source) ? $" (Source: [teal]{log.Source}[/])" : "";

        var output = $"{emoji} {timeStamp} [{levelMarkup}] {message}{source}\n";

        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
        {
            output += $"  ↳ CorrelationId: [white]{log.CorrelationId}[/]\n";
        }

        if (log.Context is { Count: > 0 })
        {
            var path = log.Context.TryGetValue("Path", out var p) ? p?.ToString() : "N/A";
            var status = log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() : "N/A";
            var duration = log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() : "N/A";

            output += $"  ↳ Path: [white]{path}[/] | Status: [white]{status}[/] | Duration: [white]{duration}ms[/]\n";

            foreach (var kvp in log.Context)
            {
                if (kvp.Key is "Path" or "StatusCode" or "DurationMs") continue;
                output += $"  ↳ [silver]{kvp.Key}[/]: [white]{kvp.Value}[/]\n";
            }
        }

        if (log.Exception is not null)
        {
            output += $"  ⚠ [red]{log.Exception.GetType().Name}: {log.Exception.Message}[/]\n";
            output += $"    [dim]{log.Exception.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "[no stack trace]"}[/]\n";
        }

        return output.TrimEnd();
    }


    /// <summary>
    /// Formats a <see cref="LogEntry"/> into a plain text string without any markup, suitable for log files or non-ANSI environments.
    /// </summary>
    /// <param name="log">The log entry to format.</param>
    /// <returns>A plain text string representation of the log entry.</returns>
    public static string FormatPlainText(LogEntry log)
    {
        var timeStamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        var level = log.Level?.ToUpperInvariant() ?? "INFO";

        var output = $"[{timeStamp}] [{level}] {log.Message}";

        if (!string.IsNullOrWhiteSpace(log.Source))
        {
            output += $" (Source: {log.Source})";
        }

        output += "\n";

        // Add correlation ID if present
        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
        {
            output += $"  ↳ CorrelationId: {log.CorrelationId}\n";
        }

        // Add standard structured fields from Context
        if (log.Context is { Count: > 0 })
        {
            var path = log.Context.TryGetValue("Path", out var p) ? p?.ToString() : "N/A";
            var status = log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() : "N/A";
            var duration = log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() : "N/A";

            output += $"  ↳ Path: {path} | Status: {status} | Duration: {duration}ms\n";

            // Add any other fields in context not already handled
            foreach (var kvp in log.Context)
            {
                if (kvp.Key is "Path" or "StatusCode" or "DurationMs")
                    continue;

                output += $"  ↳ {kvp.Key}: {kvp.Value}\n";
            }
        }

        // Add structured exception info
        if (log.Exception is not null)
        {
            output += $"  ⚠ Exception: {log.Exception.GetType().Name}: {log.Exception.Message}\n";
            output += $"    {log.Exception.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "[no stack trace]"}\n";
        }

        return output.TrimEnd();
    }

}
