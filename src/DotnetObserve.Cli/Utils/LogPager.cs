using System.Text.Json;
using DotnetObserve.Core.Models;
using Spectre.Console;

namespace DotnetObserve.Cli.Utils;

/// <summary>
/// Handles paged display of log entries to the console with optional formatting.
/// </summary>
public static class LogPager
{
    /// <summary>
    /// Displays log entries in pages of specified size, waiting for key press between pages.
    /// </summary>
    /// <param name="logs">The logs to display.</param>
    /// <param name="pageSize">Page size to show at once. If 0, displays all logs without paging.</param>
    /// <param name="jsonMode">Optional: 'pretty' or 'compact' JSON formatting.</param>
    public static void Display(IEnumerable<LogEntry> logs, int pageSize = 0, string? jsonMode = null)
    {
        var logList = logs.ToList();
        bool useJson = !string.IsNullOrWhiteSpace(jsonMode);
        bool isPretty = jsonMode?.Equals("pretty", StringComparison.OrdinalIgnoreCase) == true;

        for (int i = 0; i < logList.Count; i++)
        {
            var log = logList[i];

            if (useJson)
            {
                var json = JsonSerializer.Serialize(log, new JsonSerializerOptions
                {
                    WriteIndented = isPretty
                });
                Console.WriteLine(json);
            }
            else if (AnsiConsole.Profile.Capabilities.Ansi)
            {
                try
                {
                    AnsiConsole.MarkupLine(LogFormatter.Format(log));
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]❌ Markup error: {Markup.Escape(ex.Message)}[/]");
                    AnsiConsole.WriteLine(LogFormatter.FormatPlainText(log));
                }
            }
            else
            {
                Console.WriteLine(LogFormatter.FormatPlainText(log));
            }

            if (pageSize > 0 && (i + 1) % pageSize == 0 && i + 1 < logList.Count)
            {
                Console.WriteLine("\n-- Press any key to continue --");
                Console.ReadKey(true);
            }
        }
    }
}
