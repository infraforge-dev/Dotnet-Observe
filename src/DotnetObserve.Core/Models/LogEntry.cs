namespace DotnetObserve.Core.Models;

/// <summary>
/// Represents a structured application log event with optional context and correlation.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Unique identifier for the log entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// UTC timestamp of when the log entry was recorded.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Severity level of the log (e.g., Info, Warn, Error).
    /// </summary>
    public string Level { get; set; } = "Info";

    /// <summary>
    /// Main message describing the log event.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Logical source of the log event (e.g., API, BackgroundJob).
    /// </summary>
    public string Source { get; set; } = "App";

    /// <summary>
    /// Optional exception object for structured error details.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Optional correlation ID to link related log entries and traces.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Optional dictionary of structured key-value context fields.
    /// </summary>
    public Dictionary<string, object?> Context { get; set; } = [];
}
