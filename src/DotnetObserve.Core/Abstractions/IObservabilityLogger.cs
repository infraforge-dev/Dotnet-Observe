using DotnetObserve.Core.Models;

namespace DotnetObserve.Core.Abstractions;

/// <summary>
/// Defines a common interface for structured observability logging.
/// This abstraction allows log sinks (file, console, DB, etc.) to be swappable and testable.
/// </summary>
public interface IObservabilityLogger
{
    /// <summary>
    /// Writes a structured log entry to the underlying log sink.
    /// </summary>
    /// <param name="entry">The log entry to write.</param>
    Task LogAsync(LogEntry entry);
}
