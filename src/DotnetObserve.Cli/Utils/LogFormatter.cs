using System.Text;
using DotnetObserve.Core.Constants;
using DotnetObserve.Core.Models;
using Spectre.Console;

namespace DotnetObserve.Cli.Utils;

public static class LogFormatter
{
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

        var sb = new StringBuilder();

        var levelEscaped = Markup.Escape(log.Level ?? "");
        var messageEscaped = Markup.Escape(log.Message ?? "");
        var sourceEscaped = Markup.Escape(log.Source ?? "");

        sb.Append($"{emoji} [grey]{log.Timestamp:yyyy-MM-dd HH:mm:ss}[/] ");
        sb.Append($"[{color}]{levelEscaped}[/] ");
        sb.Append($"{messageEscaped}");

        if (!string.IsNullOrWhiteSpace(log.Source))
        {
            sb.Append($" (Source: [teal]{sourceEscaped}[/])");
        }

        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
        {
            sb.AppendLine($"  ↳ CorrelationId: [white]{Markup.Escape(log.CorrelationId)}[/]");
        }

        if (log.Context is { Count: > 0 })
        {
            var path = Markup.Escape(log.Context.TryGetValue("Path", out var p) ? p?.ToString() ?? "N/A" : "N/A");
            var status = Markup.Escape(log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() ?? "N/A" : "N/A");
            var duration = Markup.Escape(log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() ?? "N/A" : "N/A");

            sb.AppendLine($"  ↳ Path: [white]{path}[/] | Status: [white]{status}[/] | Duration: [white]{duration}ms[/]");

            foreach (var kvp in log.Context)
            {
                if (kvp.Key is "Path" or "StatusCode" or "DurationMs") continue;

                var k = Markup.Escape(kvp.Key ?? "");
                var v = Markup.Escape(kvp.Value?.ToString() ?? "");
                sb.AppendLine($"  ↳ [silver]{k}[/]: [white]{v}[/]");
            }
        }

        if (log.Exception is not null)
        {
            var exMessage = Markup.Escape(log.Exception.Message ?? "");
            var exStack = Markup.Escape(log.Exception.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "[no stack trace]");

            sb.AppendLine($"  ⚠ [red]{log.Exception.GetType().Name}: {exMessage}[/]");
            sb.AppendLine($"    [dim]{exStack}[/]");
        }

        return sb.ToString().TrimEnd();
    }

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

        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
        {
            output += $"  ↳ CorrelationId: {log.CorrelationId}\n";
        }

        if (log.Context is { Count: > 0 })
        {
            var path = log.Context.TryGetValue("Path", out var p) ? p?.ToString() : "N/A";
            var status = log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() : "N/A";
            var duration = log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() : "N/A";

            output += $"  ↳ Path: {path} | Status: {status} | Duration: {duration}ms\n";

            foreach (var kvp in log.Context)
            {
                if (kvp.Key is "Path" or "StatusCode" or "DurationMs")
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
