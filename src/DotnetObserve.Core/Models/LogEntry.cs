namespace DotnetObserve.Core.Models
{
    public class LogEntry
    {
        public Guid Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Level { get; set; } = "Info";

        public string Message { get; set; } = string.Empty;

        public string Source { get; set; } = "Application";

        public string? Exception { get; set; }

        public string? CorrelationId { get; set; }

        public Dictionary<string, object>? Context { get; set; }
    }
}