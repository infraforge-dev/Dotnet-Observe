using DotnetObserve.Core.Constants;
using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Utils
{
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

            var levelMarkup = $"[{color}]{log.Level}[/]";
            var timeStamp = $"[grey]{log.Timestamp:yyyy-MM-dd HH:mm:ss}[/]";
            var message = $"{emoji}  [bold]{log.Message}[/]";

            var output = $"\n {timeStamp} {levelMarkup} {message}";

            if (log.Context != null)
            {
                var path = log.Context.TryGetValue("Path", out var p) ? p?.ToString() : "N/A";
                var status = log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() : "N/A";
                var duration = log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() : "N/A";

                output += $"\n [grey] ↳ Path:[/] {path} [grey]| Status:[/] {status} [grey]| Duration:[/] {duration}ms";
            }

            if (log.Level == LogLevels.Error && log.Context is not null)
            {
                if (log.Context.TryGetValue("ExceptionMessage", out var exMsg))
                    output += $"\n [red]🛑 Exception:[/] {exMsg}";

                if (log.Context.TryGetValue("ExceptionLocation", out var location))
                    output += $"\n [grey] ↳ Location:[/] {location}";
            }

            return output;
        }

        public static string FormatPlainText(LogEntry log)
        {
            var timeStamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
            var level = log.Level?.ToUpperInvariant() ?? "INFO";

            var output = $"[{timeStamp}] [{level}] {log.Message}";

            if (log.Context != null)
            {
                var path = log.Context.TryGetValue("Path", out var p) ? p?.ToString() : "N/A";
                var status = log.Context.TryGetValue("StatusCode", out var s) ? s?.ToString() : "N/A";
                var duration = log.Context.TryGetValue("DurationMs", out var d) ? d?.ToString() : "N/A";

                output += $"\n  ↳ Path: {path} | Status: {status} | Duration: {duration}ms\n";
            }

            return output;
        }
    }
}