namespace DotnetObserve.Core.Models;

/// <summary>
/// Configuration settings for controlling observability behavior.
/// </summary>
public class ToolkitConfig
{
    /// <summary>
    /// Global log level (e.g., Info, Debug, Error).
    /// </summary>
    public string LogLevel { get; set; } = "Info";

    /// <summary>
    /// Enables or disables metrics collection.
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Enables or disables trace span tracking.
    /// </summary>
    public bool EnableTracing { get; set; } = true;

    /// <summary>
    /// Output target for logs and metrics (e.g., File, Console, Prometheus).
    /// </summary>
    public string Exporter { get; set; } = "File";
}
