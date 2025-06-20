namespace DotnetObserve.Cli.Rendering;
/// <summary>
/// Chooses an appropriate renderer based on CLI options.
/// </summary>
public static class RendererFactory
{
    public static ILogRenderer Create(string? jsonMode, string? format)
    {
        // Handle invalid combinations
        if (!string.IsNullOrWhiteSpace(jsonMode) && !string.IsNullOrWhiteSpace(format))
        {
            Console.WriteLine("❌ Cannot use both --json and --format simultaneously.");
            Environment.Exit(1);
        }

        if (!string.IsNullOrWhiteSpace(jsonMode))
        {
            return new JsonRenderer(jsonMode);
        }

        return format?.ToLowerInvariant() switch
        {
            "summary" => new CompactRenderer(),
            "plain" => new PlainTextRenderer(),
            _ => new AnsiRenderer()
        };
    }
}
