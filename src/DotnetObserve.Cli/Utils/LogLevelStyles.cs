using Spectre.Console;

namespace DotnetObserve.Core.Constants
{
    public class LogLevelStyles
    {
        public static readonly Dictionary<string, Color> LevelColors = new(StringComparer.OrdinalIgnoreCase)
        {
            { LogLevels.Info, Color.Green },

            { LogLevels.Warning, Color.Yellow },

            { LogLevels.Error, Color.Red },

            { LogLevels.Debug, Color.Blue },

            { LogLevels.Trace, Color.Grey },

            { LogLevels.Critical, Color.Red1 }
        };

        public static Color GetColor(string? level)
        {
            return LevelColors.TryGetValue(level ?? LogLevels.Info, out var color)
                ? color
                : Color.White;
        }
    }
}