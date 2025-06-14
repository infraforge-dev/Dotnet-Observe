namespace DotnetObserve.Core.Models
{
    public class TraceSpan
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string TraceId { get; set; } = Guid.NewGuid().ToString();

        public string SpanId { get; set; } = Guid.NewGuid().ToString();

        public string? ParentSpanId { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime EndTime { get; set; } = DateTime.UtcNow;

        public Dictionary<string, object>? Attributes { get; set; }
    }
}