using System.CommandLine;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using DotnetObserve.Core.Constants;
using DotnetObserve.Cli.Utils;
using Spectre.Console;
using System.Text.Json;

var logPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../SampleApi/logs.json"));
AnsiConsole.MarkupLine($"[grey]Looking for logs at:[/] {logPath}");

if (!File.Exists(logPath))
{
    AnsiConsole.MarkupLine("[red]❌ Log file not found![/]");
    return 1;
}

var logStore = new JsonFileStore<LogEntry>(logPath);

// Define the `tail` command
var tailCommand = new Command("tail", "Tail and display recent log entries");

var takeOption = new Option<int?>(
    aliases: new[] { "--take", "-n" },
    description: "How many log entries to show",
    getDefaultValue: () => 50
);

var levelOption = new Option<string>(
    aliases: new[] { "--level", "-l" },
    description: "Filter by log level (e.g., Info, Warn, Error)"
);

var jsonOption = new Option<string>(
    name: "--json",
    description: "Output log entries in JSON format. Options: 'pretty' or 'compact'"
);

tailCommand.AddOption(takeOption);
tailCommand.AddOption(levelOption);
tailCommand.AddOption(jsonOption);

tailCommand.SetHandler<int?, string?, string?>(
    async (int? take, string? level, string? jsonMode) =>
{
    var logs = await logStore.ReadAllAsync();

    if (!string.IsNullOrWhiteSpace(level) && !LogLevels.All.Contains(level))
    {
        AnsiConsole.MarkupLine($"[red]❌ Invalid log level:[/] {level}");
        AnsiConsole.MarkupLine($"[grey]Valid levels:[/] {string.Join(", ", LogLevels.All)}");
        return;
    }

    var filtered = logs
        .OrderByDescending(e => e.Timestamp)
        .Where(e =>
            string.IsNullOrWhiteSpace(level) ||
            (e.Level?.Equals(level, StringComparison.OrdinalIgnoreCase) ?? false))
        .Take(take ?? 50);

    var isPretty = string.Equals(jsonMode, "pretty", StringComparison.OrdinalIgnoreCase);
    var isCompact = string.Equals(jsonMode, "compact", StringComparison.OrdinalIgnoreCase);

    if (!string.IsNullOrWhiteSpace(jsonMode) && !isPretty && !isCompact)
    {
        AnsiConsole.MarkupLine("[red]❌ Invalid --json mode. Use 'pretty' or 'compact'.[/]");
    }

    foreach (var log in filtered)
    {
        if (!string.IsNullOrWhiteSpace(jsonMode))
        {
            var rawJson = JsonSerializer.Serialize(log, new JsonSerializerOptions
            {
                WriteIndented = isPretty
            });

            Console.WriteLine(rawJson);
        }
        else if (AnsiConsole.Profile.Capabilities.Ansi)
        {
            AnsiConsole.MarkupLine(LogFormatter.Format(log));
        }
        else
        {
            Console.WriteLine(LogFormatter.FormatPlainText(log));
        }
    }
}, takeOption, levelOption, jsonOption);

// Add commands to root and run
var rootCommand = new RootCommand("dotnet-observe CLI tool");
rootCommand.AddCommand(tailCommand);

return await rootCommand.InvokeAsync(args);
