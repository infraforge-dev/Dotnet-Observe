using Spectre.Console;

namespace DotnetObserve.Core.Constants;

/// <summary>
/// Provides ANSI color mappings and lookup utilities for log levels used in CLI formatting.
/// </summary>
public class LogLevelStyles
{
    /// <summary>
    /// A mapping of log levels to corresponding <see cref="Color"/> values used for terminal output.
    /// </summary>
    public static readonly Dictionary<string, Color> LevelColors = new(StringComparer.OrdinalIgnoreCase)
    {
        { LogLevels.Info, Color.Green },

        { LogLevels.Warning, Color.Yellow },

        { LogLevels.Error, Color.Red },

        { LogLevels.Debug, Color.Blue },

        { LogLevels.Trace, Color.Grey },

        { LogLevels.Critical, Color.Red1 }
    };

    /// <summary>
    /// Gets the ANSI color associated with a given log level.
    /// </summary>
    /// <param name="level">The log level as a string (e.g., "Info", "Error").</param>
    /// <returns>The <see cref="Color"/> assigned to the specified level, or <see cref="Color.White"/> if unknown.</returns>
    public static Color GetColor(string? level)
    {
        return LevelColors.TryGetValue(level ?? LogLevels.Info, out var color)
            ? color
            : Color.White;
    }
}
