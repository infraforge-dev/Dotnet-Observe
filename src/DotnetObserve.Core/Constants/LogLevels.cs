namespace DotnetObserve.Core.Constants;

/// <summary>
/// Defines common log level names used throughout the observability toolkit.
/// Provides both individual level constants and a full set for validation or enumeration.
/// </summary>
public static class LogLevels
{
    /// <summary>
    /// Informational messages indicating normal operation.
    /// </summary>
    public const string Info = "Info";

    /// <summary>
    /// Warnings about potential issues that are not necessarily errors.
    /// </summary>
    public const string Warning = "Warning";

    /// <summary>
    /// Errors that indicate a failure in the current operation.
    /// </summary>
    public const string Error = "Error";

    /// <summary>
    /// Debug-level messages intended for development and debugging purposes.
    /// </summary>
    public const string Debug = "Debug";

    /// <summary>
    /// Trace-level messages that provide highly detailed information, typically for diagnostics.
    /// </summary>
    public const string Trace = "Trace";

    /// <summary>
    /// Critical errors indicating system-wide failures or crashes.
    /// </summary>
    public const string Critical = "Critical";

    /// <summary>
    /// A collection of all supported log levels. Useful for validation, filtering, and dropdowns.
    /// </summary>
    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        Info,
        Warning,
        Error,
        Debug,
        Trace,
        Critical
    };
}
