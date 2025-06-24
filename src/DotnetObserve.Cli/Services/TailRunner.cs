using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using Spectre.Console;
using DotnetObserve.Cli.Rendering;
using DotnetObserve.Cli.Utils;
using LogPager = DotnetObserve.Cli.Paging.LogPager;

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
        /// <param name="takeOption">Maximum number of log entries to display.</param>
        /// <param name="levelOption">Log level filter (Info, Warn, Error, etc.).</param>
        /// <param name="jsonOption">Output format: 'pretty' or 'compact'. Null if --json is not passed.</param>
        /// <param name="sinceOption">Only include logs after this timestamp.</param>
        /// <param name="containsOption">Filter logs containing this keyword.</param>
        /// <param name="pageSizeOption">How many logs to show per page.</param>
        /// <param name="formatOption">Optional CLI format (summary/plain).</param>
        public static async Task Execute(int takeOption, string levelOption, string jsonOption,
    DateTimeOffset? sinceOption, string containsOption, int pageSizeOption)
        {
            var logPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../SampleApi/test_logs_mixed.json"));
            if (!File.Exists(logPath))
            {
                AnsiConsole.MarkupLine("[red]❌ Log file not found![/]");
                return;
            }

            var logStore = new JsonFileStore<LogEntry>(logPath);
            var logs = await logStore.ReadAllAsync();

            logs = LogFilter.Apply(logs, levelOption, sinceOption, containsOption)
                .OrderByDescending(log => log.Timestamp)
                .Take(takeOption)
                .ToList();

            if (logs.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]⚠️ No log entries matched the filter criteria.[/]");
                return;
            }

            ILogRenderer renderer = new JsonRenderer(jsonOption); 

            LogPager.Display(logs, pageSizeOption, renderer);
        }
    }
}
