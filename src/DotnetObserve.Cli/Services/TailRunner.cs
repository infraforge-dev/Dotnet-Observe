using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using DotnetObserve.Cli.Utils;
using Spectre.Console;

namespace DotnetObserve.Cli.Commands
{
    /// <summary>
    /// Executes the logic for the 'tail' command.
    /// Loads, filters, and displays recent log entries from the log store.
    /// </summary>
    public static class TailRunner
    {
        /// <summary>
        /// Executes the 'tail' command with the provided options.
        /// </summary>
        /// <param name="take">Maximum number of log entries to display.</param>
        /// <param name="level">Log level filter (Info, Warn, Error, etc.).</param>
        /// <param name="json">Output format: 'pretty' or 'compact'.</param>
        /// <param name="since">Only include logs after this timestamp.</param>
        /// <param name="contains">Filter logs containing this keyword.</param>
        public static async Task Execute(int? take, string level, string json, DateTimeOffset? since, string contains, int pageSize)
        {
            var logPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../SampleApi/logs.json"));
            if (!File.Exists(logPath))
            {
                AnsiConsole.MarkupLine("[red]❌ Log file not found![/]");
                return;
            }

            var logStore = new JsonFileStore<LogEntry>(logPath);
            var logs = await logStore.ReadAllAsync();

            logs = LogFilter.Apply(logs, level, since, contains)
                .OrderByDescending(log => log.Timestamp)
                .Take(take ?? 50)
                .ToList();

            if (logs.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]⚠️ No log entries matched the filter criteria.[/]");
                return;
            }

            LogPager.Display(logs, pageSize, jsonMode: json);
        }
    }
}
