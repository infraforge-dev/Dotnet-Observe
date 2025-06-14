namespace DotnetObserve.Core.Models;

/// <summary>
/// Represents a recorded metric such as duration, count, or error rate.
/// </summary>
public class MetricEntry
{
    /// <summary>
    /// Unique identifier for the metric entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// UTC timestamp when the metric was recorded.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Name of the metric (e.g., RequestDuration, ErrorRate).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Value of the metric.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Optional unit of measurement (e.g., ms, %, count).
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Optional tagging information for filtering or aggregation.
    /// </summary>
    public string? Tags { get; set; }
}
