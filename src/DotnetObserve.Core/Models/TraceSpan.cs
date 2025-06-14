namespace DotnetObserve.Core.Models;

/// <summary>
/// Represents a span within a trace, used for tracking the execution of an operation.
/// </summary>
public class TraceSpan
{
    /// <summary>
    /// Unique identifier for this trace span instance.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Shared identifier for all spans in the same trace.
    /// </summary>
    public string TraceId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Unique identifier for this span.
    /// </summary>
    public string SpanId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Identifier of the parent span, if applicable.
    /// </summary>
    public string? ParentSpanId { get; set; }

    /// <summary>
    /// Descriptive name of the span (e.g., "GET /api/orders").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the span started.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the span ended.
    /// </summary>
    public DateTime EndTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional key-value attributes describing the span context.
    /// </summary>
    public Dictionary<string, object>? Attributes { get; set; }
}
