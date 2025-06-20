using DotnetObserve.Core.Models;
using Spectre.Console;

namespace DotnetObserve.Cli.Utils
{
    /// <summary>
    /// Provides methods for rendering log entries in a stylized plain-text format using Spectre.Console.
    /// </summary>
    public static class LogPrinter
    {
        /// <summary>
        /// Prints a single <see cref="LogEntry"/> to the console in a human-readable format.
        /// </summary>
        /// <param name="log">The log entry to display.</param>
        public static void Print(LogEntry log)
        {
            var formatted = LogFormatter.Format(log);

            try
            {
                foreach (var line in formatted.Split('\n'))
                {
                    AnsiConsole.MarkupLine(line.TrimEnd());
                }

                AnsiConsole.WriteLine(); 
                AnsiConsole.Write(new Rule().RuleStyle("grey"));
                AnsiConsole.WriteLine(); 
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Markup error: {Markup.Escape(ex.Message)}[/]");
                Console.WriteLine(LogFormatter.FormatPlainText(log));
            }
        }


    }
}
