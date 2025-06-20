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
    public static void Display(IEnumerable<LogEntry> logs, int pageSize, string? jsonMode = null)
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

                if (isPretty && AnsiConsole.Profile.Capabilities.Ansi)
                {
                    // Pretty-print with colorized keys and values
                    var lines = json.Split('\n');

                    // Precompute max key length for alignment
                    int maxKeyLength = lines
                        .Select(l => l.IndexOf(':'))
                        .Where(i => i > 0)
                        .DefaultIfEmpty(0)
                        .Max();

                    foreach (var line in lines)
                    {
                        var idx = line.IndexOf(':');
                        if (idx > 0)
                        {
                            var rawKey = line.Substring(0, idx).TrimEnd();
                            var rawValue = line.Substring(idx + 1);

                            var paddedKey = rawKey.PadRight(maxKeyLength);
                            var escapedKey = Markup.Escape(paddedKey);
                            var escapedVal = Markup.Escape(rawValue);

                            AnsiConsole.MarkupLine($"[grey]{escapedKey}[/]:[white]{escapedVal}[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine(Markup.Escape(line));
                        }
                    }
                }
                else
                {
                    // Compact or plain JSON
                    Console.WriteLine(json);
                }
            }
            else if (AnsiConsole.Profile.Capabilities.Ansi)
            {
                try
                {
                    LogPrinter.Print(log);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]❌ Markup error: {Markup.Escape(ex.Message)}[/]");
                    Console.WriteLine(LogFormatter.FormatPlainText(log));
                }
            }
            else
            {
                Console.WriteLine(LogFormatter.FormatPlainText(log));
            }

            // Handle paging
            if (pageSize > 0 && (i + 1) % pageSize == 0)
            {
                if (i + 1 < logList.Count)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[grey]-- Press any key to continue --[/]");
                    Console.ReadKey(true);
                }
                else
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[grey](End of logs)[/]");
                }
            }
        }
    }
}
