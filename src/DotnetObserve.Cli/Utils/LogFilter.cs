using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Utils
{
    /// <summary>
    /// Provides filtering logic for collections of <see cref="LogEntry"/> objects based on log level, time, and other criteria.
    /// </summary>
    public static class LogFilter
    {
        /// <summary>
        /// Applies filter criteria to a list of logs, such as log level, timestamp, and keyword matching.
        /// </summary>
        /// <param name="logs">The collection of log entries to filter.</param>
        /// <param name="level">Optional log level to filter by (e.g., "Error", "Info").</param>
        /// <param name="since">Optional timestamp. Only logs with a timestamp after this value will be returned.</param>
        /// <param name="contains">Optional search keyword. Only logs that contain this keyword in the message or context will be returned.</param>
        /// <returns>A filtered enumerable of <see cref="LogEntry"/> objects.</returns>
        public static IEnumerable<LogEntry> Apply(
            IEnumerable<LogEntry> logs,
            string? level = null,
            DateTimeOffset? since = null,
            string? contains = null)
        {
            return logs.Where(log =>
            {
                var levelMatch = string.IsNullOrWhiteSpace(level)
                    || log.Level?.Equals(level, StringComparison.OrdinalIgnoreCase) == true;

                var sinceMatch = !since.HasValue
                    || log.Timestamp >= since.Value;

                var containsMatch = string.IsNullOrWhiteSpace(contains)
                    || (log.Message?.Contains(contains, StringComparison.OrdinalIgnoreCase) == true)
                    || (log.Context?.Any(kv => kv.Value?.ToString()?.Contains(contains, StringComparison.OrdinalIgnoreCase) == true) == true);

                return levelMatch && sinceMatch && containsMatch;
            });
        }
    }
}
