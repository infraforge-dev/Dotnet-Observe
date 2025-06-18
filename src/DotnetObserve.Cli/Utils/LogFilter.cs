using DotnetObserve.Core.Models;

namespace DotnetObserve.Cli.Utils
{
    /// <summary>
    /// Provides filtering logic for collections of <see cref="LogEntry"/> objects based on log level, time, and other criteria.
    /// </summary>
    public class LogFilter
    {
        /// <summary>
        /// Applies filter criteria to a list of logs, such as log level and minimum timestamp.
        /// </summary>
        /// <param name="logs">The collection of log entries to filter.</param>
        /// <param name="level">Optional log level to filter by (e.g., "Error", "Info").</param>
        /// <param name="since">Optional timestamp. Only logs with a timestamp after this value will be returned.</param>
        /// <returns>A filtered enumerable of <see cref="LogEntry"/> objects.</returns>
        public static IEnumerable<LogEntry> Apply(
            IEnumerable<LogEntry> logs,
            string? level = null,
            DateTimeOffset? since = null)
        {
            return logs
                .Where(log =>
                (string.IsNullOrWhiteSpace(level) ||
                    log.Level?.Equals(level, StringComparison.OrdinalIgnoreCase) == true) &&
                (!since.HasValue || log.Timestamp >= since.Value));
        }

    }
}