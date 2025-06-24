namespace DotnetObserve.Cli.Rendering;

/// <summary>
/// Chooses an appropriate renderer based on CLI format options.
/// JSON rendering is handled directly in TailRunner.
/// </summary>
public static class RendererFactory
{
    /// <summary>
    /// Selects a renderer based on the provided output format.
    /// </summary>
    /// <param name="format">The output format (e.g. "summary", "plain").</param>
    /// <returns>The appropriate <see cref="ILogRenderer"/> instance.</returns>
    public static ILogRenderer Create(string? format)
    {
        return format?.ToLowerInvariant() switch
        {
            "summary" => new CompactRenderer(),
            "plain" => new PlainTextRenderer(),
            _ => new AnsiRenderer()
        };
    }
}
