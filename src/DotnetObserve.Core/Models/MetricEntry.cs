namespace DotnetObserve.Core.Models
{
    public class MetricEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Name { get; set; } = string.Empty; // e.g., RequestDuration

        public double Value { get; set; }

        public string? Unit { get; set; } // ms, %, count, etc.

        public string? Tags { get; set; } // e.g., endpoint=/api/orders;status=200
    }
}