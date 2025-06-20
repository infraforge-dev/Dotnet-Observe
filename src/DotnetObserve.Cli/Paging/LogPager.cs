using DotnetObserve.Core.Models;
using DotnetObserve.Cli.Rendering;

namespace DotnetObserve.Cli.Paging;

/// <summary>
/// Handles paged display of log entries to the console using a provided renderer.
/// </summary>
public static class LogPager
{
    /// <summary>
    /// Displays log entries in pages of specified size using the given renderer.
    /// </summary>
    /// <param name="logs">The logs to display.</param>
    /// <param name="pageSize">Page size to show at once. If 0, displays all logs without paging.</param>
    /// <param name="renderer">The renderer to use for log output.</param>
    public static void Display(IEnumerable<LogEntry> logs, int pageSize, ILogRenderer renderer)
    {
        var logList = logs.ToList();

        for (int i = 0; i < logList.Count; i++)
        {
            renderer.Render(logList[i]);

            if (pageSize > 0 && (i + 1) % pageSize == 0)
            {
                if (i + 1 < logList.Count)
                {
                    Console.WriteLine();
                    Console.Write("[grey]-- Press any key to continue --[/]");
                    Console.ReadKey(true);
                    Console.WriteLine();
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                    Console.Write("[grey](End of logs)[/]");
                }
            }
        }
    }
}
