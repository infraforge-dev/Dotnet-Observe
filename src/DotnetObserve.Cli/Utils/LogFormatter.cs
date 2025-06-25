using System.Text;
using DotnetObserve.Core.Constants;
using DotnetObserve.Core.Models;
using Spectre.Console;

namespace DotnetObserve.Cli.Utils;

/// <summary>
/// Provides methods to convert <see cref="LogEntry"/> objects into formatted strings
/// for terminal display with Spectre.Console markup.
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
        var sb = new StringBuilder();

        sb.AppendLine(FormatHeader(log));
        sb.AppendLine(FormatMessage(log));

        if (!string.IsNullOrWhiteSpace(log.Source))
            sb.AppendLine(FormatSource(log));

        if (!string.IsNullOrWhiteSpace(log.CorrelationId))
            sb.AppendLine(FormatCorrelationId(log));

        sb.AppendLine(FormatMetadata(log));

        sb.AppendLine(FormatExceptionContext(log));

        if (log.Exception is not null)
            sb.AppendLine(FormatRawException(log));

        sb.Append(FormatRemainingContext(log));

        return sb.ToString().TrimEnd();
    }

    private static string FormatHeader(LogEntry log)
    {
        var color = LogLevelStyles.GetColor(log.Level);
        var emoji = log.Level switch
        {
            "Error" => "❌",
            "Warning" or "Warn" => "⚠️ ",
            "Info" => "ℹ️ ",
            "Debug" => "🐛 ",
            "Trace" => "🔍 ",
            _ => ""
        };

        var levelEscaped = Markup.Escape(log.Level ?? "");
        return $"{emoji} [grey]{log.Timestamp:yyyy-MM-dd HH:mm:ss}[/] [{color}]{levelEscaped}[/]";
    }

    private static string FormatMessage(LogEntry log) =>
        $"[bold green3_1]Message:[/] {Markup.Escape(log.Message ?? "")}";

    private static string FormatSource(LogEntry log) =>
        $"[yellow2]Source:[/] [white]{Markup.Escape(log.Source ?? "")}[/]";

    private static string FormatCorrelationId(LogEntry log) =>
        $"[cyan]CorrelationId:[/] [white]{Markup.Escape(log.CorrelationId ?? "")}[/]";

    private static string FormatMetadata(LogEntry log)
    {
        var ctx = log.Context ?? new();
        var path = Markup.Escape(ctx.TryGetValue("Path", out var p) ? p?.ToString() ?? "N/A" : "N/A");
        var status = Markup.Escape(ctx.TryGetValue("StatusCode", out var s) ? s?.ToString() ?? "N/A" : "N/A");
        var duration = Markup.Escape(ctx.TryGetValue("DurationMs", out var d) ? d?.ToString() ?? "N/A" : "N/A");

        return $"[green]Path:[/] [white]{path}[/] | [green]Status:[/] [white]{status}[/] | [green]Duration:[/] [white]{duration}ms[/]";
    }

    private static string FormatExceptionContext(LogEntry log)
    {
        var sb = new StringBuilder();
        var ctx = log.Context ?? new();

        if (ctx.TryGetValue("ExceptionType", out var exType))
            sb.AppendLine($"[red]ExceptionType:[/] [white]{Markup.Escape(exType?.ToString() ?? "")}[/]");

        if (ctx.TryGetValue("ExceptionLocation", out var exLoc))
            sb.AppendLine($"[dim]ExceptionLocation:[/] [white]{Markup.Escape(exLoc?.ToString() ?? "")}[/]");

        return sb.ToString();
    }

    private static string FormatRawException(LogEntry log)
    {
        var ex = log.Exception!;
        var exMessage = Markup.Escape(ex.Message ?? "");
        var exStack = Markup.Escape(ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "[no stack trace]");

        return $"[red]ExceptionType:[/] [white]{ex.GetType().Name}[/] - [white]{exMessage}[/]\n[dim]ExceptionLocation:[/] [white]{exStack}[/]";
    }

    private static string FormatRemainingContext(LogEntry log)
    {
        var sb = new StringBuilder();
        var knownKeys = new HashSet<string> { "Path", "StatusCode", "DurationMs", "ExceptionType", "ExceptionLocation", "Method" };

        foreach (var kvp in log.Context ?? Enumerable.Empty<KeyValuePair<string, object?>>())
        {
            if (knownKeys.Contains(kvp.Key ?? "")) continue;

            var k = Markup.Escape(kvp.Key ?? "");
            var v = Markup.Escape(kvp.Value?.ToString() ?? "");
            sb.AppendLine($"  [silver]{k}:[/] [white]{v}[/]");
        }

        return sb.ToString();
    }


}
