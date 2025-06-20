using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Rendering;

/// <summary>
/// Defines a contract for rendering log entries in various output formats.
/// </summary>
public interface ILogRenderer
{
    /// <summary>
    /// Renders a single log entry to the console or output stream.
    /// </summary>
    /// <param name="log">The log entry to render.</param>
    void Render(LogEntry log);
}
