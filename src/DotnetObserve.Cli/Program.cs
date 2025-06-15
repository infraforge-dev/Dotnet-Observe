using System.CommandLine;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;

var logPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../SampleApi/logs.json"));

var logStore = new JsonFileStore<LogEntry>(logPath);

var tailCommand = new Command("tail", "Tail and display recent log entries");

// Define options as separate variables - In System.CommandLine v2.0, SetHandler uses strongly typed overloads, and it can't
// infer argument types from Options[i].You must pass the Option<T> instance you declared.
var takeOption = new Option<int?>(
    new[] { "--take", "-n" },
    () => 50,
    "How many logs to show");

var levelOption = new Option<string>(
    new[] { "--level", "-l" },
    "Filter by log level (e.g., Info, Warn, Error)");

tailCommand.AddOption(takeOption);
tailCommand.AddOption(levelOption);

tailCommand.SetHandler(async (int? take, string level) =>
{
    var logs = await logStore.ReadAllAsync();

    var filtered = logs
        .OrderByDescending(e => e.Timestamp)
        .Where(e => string.IsNullOrWhiteSpace(level) || e.Level.Equals(level, StringComparison.OrdinalIgnoreCase))
        .Take(take ?? 50);

    foreach (var log in filtered)
    {
        Console.WriteLine($"[{log.Timestamp:O}] [{log.Level}] {log.Message}");
    }
}, takeOption, levelOption);

var root = new RootCommand("dotnet-observe CLI tool");
root.AddCommand(tailCommand);

return await root.InvokeAsync(args);