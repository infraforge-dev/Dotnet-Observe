using DotnetObserve.Cli.Utils;
using DotnetObserve.Core.Models;
using Spectre.Console;

namespace DotnetObserve.Cli.Rendering;

/// <summary>
/// Renders log entries using Spectre.Console with ANSI styling.
/// </summary>
public class AnsiRenderer : ILogRenderer
{
    public void Render(LogEntry log)
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
            Console.WriteLine(log.Message);
        }
    }
}
