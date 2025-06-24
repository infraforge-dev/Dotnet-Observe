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

            var takeOption = new Option<int>(
                ["--take", "-n"],
                description: "How many log entries to show",
                getDefaultValue: () => 50
            );

            var levelOption = new Option<string>(
                ["--level", "-l"],
                description: "Filter by log level (e.g., Info, Warn, Error)"
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            levelOption.SetDefaultValue(null);

            var jsonOption = new Option<string>(
                ["--json", "-j"],
                description: "Output log entries in JSON format. Must be 'pretty' or 'compact'."
            )
            {
                Arity = ArgumentArity.ExactlyOne
            };

            var sinceOption = new Option<DateTimeOffset?>(
                "--since",
                description: "Only include logs after this UTC timestamp (e.g. 2025-06-17T08:00:00Z)"
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            sinceOption.SetDefaultValue(null);

            var containsOption = new Option<string>(
                "--contains",
                description: "Only show logs that contain this keyword in the message"
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            containsOption.SetDefaultValue("");

            var pageSizeOption = new Option<int>(
                ["--page-size"],
                description: "How many logs to show per page (0 = no paging)",
                getDefaultValue: () => 20
            );

            levelOption.AddValidator(result =>
            {
                var val = result.GetValueOrDefault<string>()?.ToLowerInvariant();

                if ((!string.IsNullOrEmpty(val)) &&
                    (val is not ("info" or "warn" or "warning" or "error" or "debug" or "trace" or "critical")))
                {
                    result.ErrorMessage = "Valid log levels: Info, Warn, Error, Debug, Trace, Critical";
                }
            });

            jsonOption.AddValidator(result =>
            {
                var val = result.GetValueOrDefault<string>()?.ToLowerInvariant();
                if (val is not ("pretty" or "compact"))
                    result.ErrorMessage = "Valid values for --json: pretty, compact";
            });

            cmd.AddOption(takeOption);
            cmd.AddOption(levelOption);
            cmd.AddOption(jsonOption);
            cmd.AddOption(sinceOption);
            cmd.AddOption(containsOption);
            cmd.AddOption(pageSizeOption);

            cmd.SetHandler(
           TailRunner.Execute,
           takeOption, levelOption, jsonOption, sinceOption, containsOption, pageSizeOption
       );

            return cmd;
        }
    }
}
