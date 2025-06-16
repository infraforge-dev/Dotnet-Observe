namespace DotnetObserve.Core.Constants
{
    public static class LogLevels
    {
        public const string Info = "Info";

        public const string Warning = "Warning";

        public const string Error = "Error";

        public const string Debug = "Debug";

        public const string Trace = "Trace";

        public const string Critical = "Critical";

        /// <summary>
        /// All log levels, useful for validation or dropdowns.
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
}