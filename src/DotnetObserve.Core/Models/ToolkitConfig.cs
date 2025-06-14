namespace DotnetObserve.Core
{
    public class ToolkitConfig
    {
        public string LogLevel { get; set; } = "Info"; // Info, Debug, Warn, Error

        public bool EnableMetrics { get; set; } = true;

        public bool EnableTracing { get; set; } = true;

        public string Exporter { get; set; } = "File"; // Console, File, Prometheus, etc.
    }
}