using System.Text.Json;
using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Rendering;

/// <summary>
/// Renders log entries as JSON (pretty or compact).
/// </summary>
public class JsonRenderer : ILogRenderer
{
    private readonly bool _pretty;

    public JsonRenderer(string mode)
    {
        _pretty = mode?.Equals("pretty", StringComparison.OrdinalIgnoreCase) == true;
    }

    public void Render(LogEntry log)
    {
        var json = JsonSerializer.Serialize(log, new JsonSerializerOptions
        {
            WriteIndented = _pretty
        });

        Console.WriteLine(json);
    }
}
