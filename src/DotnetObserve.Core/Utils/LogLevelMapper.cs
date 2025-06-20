using DotnetObserve.Core.Constants;

namespace DotnetObserve.Core.Utils;

/// <summary>
/// Maps HTTP status codes to string-based log levels used by the observability toolkit.
/// </summary>
public static class LogLevelMapper
{
    private static readonly Dictionary<int, string> StatusOverrides = new()
    {
        [400] = LogLevels.Info,
        [401] = LogLevels.Warning,
        [403] = LogLevels.Warning,
        [404] = LogLevels.Info,
        [429] = LogLevels.Warning,
        [500] = LogLevels.Error,
        [502] = LogLevels.Error,
        [503] = LogLevels.Error,
        [504] = LogLevels.Error
    };

    /// <summary>
    /// Returns the log level string for the given HTTP status code.
    /// </summary>
    public static string MapStatusToLogLevel(int statusCode)
    {
        if (StatusOverrides.TryGetValue(statusCode, out var level))
            return level;

        return statusCode switch
        {
            >= 500 => LogLevels.Error,
            >= 400 => LogLevels.Info,
            _ => LogLevels.Info
        };
    }

    public static void SetOverride(int statusCode, string logLevel)
    {
        if (LogLevels.All.Contains(logLevel))
            StatusOverrides[statusCode] = logLevel;
    }
}
