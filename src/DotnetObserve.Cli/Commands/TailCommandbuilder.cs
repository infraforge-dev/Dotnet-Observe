using System.CommandLine;

namespace DotnetObserve.Cli.Commands
{
    /// <summary>
    /// Provides the builder method for the 'tail' CLI command.
    /// Responsible for setting up options and linking to the handler logic.
    /// </summary>
    public static class TailCommandBuilder
    {
        /// <summary>
        /// Builds the 'tail' command with all supported options and sets its handler.
        /// </summary>
        /// <returns>The constructed <see cref="Command"/> instance for 'tail'.</returns>
        public static Command Build()
        {
            var cmd = new Command("tail", "Tail and display recent log entries");

            var take = new Option<int>(
                ["--take", "-n"],
                description: "How many log entries to show",
                getDefaultValue: () => 50
            );

            var level = new Option<string>(
                ["--level", "-l"],
                description: "Filter by log level (e.g., Info, Warn, Error)"
            )
            {
                Arity = ArgumentArity.ExactlyOne
            };
            level.SetDefaultValue(null);

            var json = new Option<string>(
                "--json",
                description: "Output log entries in JSON format. Options: 'pretty' or 'compact'"
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            json.SetDefaultValue("");

            var since = new Option<DateTimeOffset?>(
                "--since",
                description: "Only include logs after this UTC timestamp (e.g. 2025-06-17T08:00:00Z)"
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            since.SetDefaultValue(null);

            var contains = new Option<string>(
                "--contains",
                description: "Only show logs that contain this keyword in the message"
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            contains.SetDefaultValue("");

            var pageSize = new Option<int>(
                ["--page-size"],
                description: "How many logs to show per page (0 = no paging)",
                getDefaultValue: () => 20
            );

            var format = new Option<string?>(
                ["--format", "-f"],
                description: "Output format mode (e.g., 'summary')"
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            format.SetDefaultValue(null);

            level.AddValidator(result =>
            {
                var val = result.GetValueOrDefault<string>()?.ToLowerInvariant();

                if ((!string.IsNullOrEmpty(val)) &&
                    (val is not ("info" or "warn" or "warning" or "error" or "debug" or "trace" or "critical")))
                {
                    result.ErrorMessage = "Valid log levels: Info, Warn, Error, Debug, Trace, Critical";
                }
            });

            json.AddValidator(result =>
            {
                var val = result.GetValueOrDefault<string>()?.ToLowerInvariant();

                if (!string.IsNullOrEmpty(val) && val is not ("pretty" or "compact"))
                    result.ErrorMessage = "Valid values for --json: pretty, compact";
            });

            format.AddValidator(result =>
            {
                var val = result.GetValueOrDefault<string>()?.ToLowerInvariant();

                if (!string.IsNullOrEmpty(val) && val is not ("summary" or "plain"))
                    result.ErrorMessage = "Valid values for --format: summary, plain";
            });


            cmd.AddOption(take);
            cmd.AddOption(level);
            cmd.AddOption(json);
            cmd.AddOption(since);
            cmd.AddOption(contains);
            cmd.AddOption(pageSize);
            cmd.AddOption(format);

            cmd.SetHandler(TailRunner.Execute, take, level, json, since, contains, pageSize, format);

            return cmd;
        }
    }
}
