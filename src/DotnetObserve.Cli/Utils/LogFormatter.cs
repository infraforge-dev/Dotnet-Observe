using System.Text;
using DotnetObserve.Core.Constants;
using DotnetObserve.Core.Models;
using Spectre.Console;

namespace DotnetObserve.Cli.Utils;

/// <summary>
/// Provides methods to convert <see cref="LogEntry"/> objects into formatted strings
/// for terminal display, either with Spectre.Console markup or plain text fallback.
/// </summary>
public static class LogFormatter
{
    /// <summary>
    /// Formats a <see cref="LogEntry"/> with ANSI color and Spectre markup for rich terminal output.
    /// </summary>
    /// <param name="log">The log entry to format.</param>
    /// <returns>A markup-formatted string ready for Spectre.Console rendering.</returns>
    public static string Format(LogEntry log)
    {
        // Map log level to color and emoji
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

        var sb = new StringBuilder();

        // Escape markup-sensitive strings to avoid rendering issues
        var levelEscaped = Markup.Escape(log.Level ?? "");
        var messageEscaped = Markup.Escape(log.Message ?? "");
        var sourceEscaped = Markup.Escape(log.Source ?? "");

        // Header: emoji + timestamp + log level
        sb.AppendLine($"{emoji} [grey]{log.Timestamp:yyyy-MM-dd HH:mm:ss}[/] [{color}]{levelEscaped}[/]");

        // Log message
        sb.AppendLine($"[bold green3_1]Message:[/] {messageEscaped}");

        // Optional source field
        if (!string.IsNullOrWhiteSpace(log.Source))
        {
            sb.AppendLine($"[yellow2]Source:[/] [white]{sourceEscaped}[/]");
        }

        // Optional correlation ID
        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
        {
            sb.AppendLine($"[cyan]CorrelationId:[/] [white]{Markup.Escape(log.CorrelationId)}[/]");
        }

        // Request metadata: Path, StatusCode, DurationMs
        if (log.Context is { Count: > 0 })
        {
            var path = Markup.Escape(log.Context.TryGetValue("Path", out var p) ? p?.ToString() ?? "N/A" : "N/A");
            var status = Markup.Escape(log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() ?? "N/A" : "N/A");
            var duration = Markup.Escape(log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() ?? "N/A" : "N/A");

            sb.AppendLine($"[green]Path:[/] [white]{path}[/] | [green]Status:[/] [white]{status}[/] | [green]Duration:[/] [white]{duration}ms[/]");
        }

        // Special styling for ExceptionType and ExceptionLocation (if present in context)
        if (log.Context?.TryGetValue("ExceptionType", out var exType) == true)
        {
            var typeText = Markup.Escape(exType?.ToString() ?? "");
            sb.AppendLine($"[red]ExceptionType:[/] [white]{typeText}[/]");
        }

        if (log.Context?.TryGetValue("ExceptionLocation", out var exLoc) == true)
        {
            var locText = Markup.Escape(exLoc?.ToString() ?? "");
            sb.AppendLine($"[dim]ExceptionLocation:[/] [white]{locText}[/]");
        }

        // Render all remaining context values not already handled
        foreach (var kvp in log.Context ?? Enumerable.Empty<KeyValuePair<string, object?>>())
        {
            if (kvp.Key is "Path" or "StatusCode" or "DurationMs" or "ExceptionType" or "ExceptionLocation")
                continue;

            var k = Markup.Escape(kvp.Key ?? "");
            var v = Markup.Escape(kvp.Value?.ToString() ?? "");
            sb.AppendLine($"  [silver]{k}:[/] [white]{v}[/]");
        }

        // Fallback for unhandled Exception object (non-context-based)
        if (log.Exception is not null)
        {
            var exMessage = Markup.Escape(log.Exception.Message ?? "");
            var exStack = Markup.Escape(log.Exception.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "[no stack trace]");

            sb.AppendLine($"[red]ExceptionType:[/] [white]{log.Exception.GetType().Name}[/] - [white]{exMessage}[/]");
            sb.AppendLine($"[dim]ExceptionLocation:[/] [white]{exStack}[/]");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Formats a <see cref="LogEntry"/> as plain text for non-color terminals or log file output.
    /// </summary>
    /// <param name="log">The log entry to format.</param>
    /// <returns>A simple, human-readable string representation of the log.</returns>
    public static string FormatPlainText(LogEntry log)
    {
        // Timestamp and uppercase log level
        var timeStamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        var level = log.Level?.ToUpperInvariant() ?? "INFO";

        // Start with main log line
        var output = $"[{timeStamp}] [{level}] {log.Message}";

        // Optional source
        if (!string.IsNullOrWhiteSpace(log.Source))
        {
            output += $" (Source: {log.Source})";
        }

        output += "\n";

        // Optional correlation ID
        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
        {
            output += $"  ↳ CorrelationId: {log.CorrelationId}\n";
        }

        // Structured context metadata
        if (log.Context is { Count: > 0 })
        {
            var path = log.Context.TryGetValue("Path", out var p) ? p?.ToString() : "N/A";
            var status = log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() : "N/A";
            var duration = log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() : "N/A";

            output += $"  ↳ Path: {path} | Status: {status} | Duration: {duration}ms\n";

            // Remaining context values
            foreach (var kvp in log.Context)
            {
                if (kvp.Key is "Path" or "StatusCode" or "DurationMs")
                    continue;

                output += $"  ↳ {kvp.Key}: {kvp.Value}\n";
            }
        }

        // Append exception details if present
        if (log.Exception is not null)
        {
            output += $"  ⚠ Exception: {log.Exception.GetType().Name}: {log.Exception.Message}\n";
            output += $"    {log.Exception.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "[no stack trace]"}\n";
        }

        return output.TrimEnd();
    }
}
