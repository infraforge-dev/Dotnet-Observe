using DotnetObserve.Core.Models;
using DotnetObserve.Cli.Utils;
using FluentAssertions;

namespace DotnetObserve.Cli.Tests;

public class LogFormatterTests
{
    [Fact]
    public void Format_ShouldRenderBasicLogEntry_WithEmojiAndColors()
    {
        var entry = new LogEntry
        {
            Timestamp = new DateTime(2025, 6, 18, 12, 0, 0, DateTimeKind.Utc),
            Level = "Info",
            Message = "Application started",
            Source = "CLI"
        };

        var result = LogFormatter.Format(entry);

        result.Should().Contain("ℹ️");
        result.Should().Contain("[green]Info[/]");
        result.Should().Contain("Application started");
        result.Should().Contain("[yellow2]Source:[/]");
    }

    [Fact]
    public void Format_ShouldIncludeContextFields_AndHighlightStandardOnes()
    {
        var entry = new LogEntry
        {
            Message = "User login",
            Context = new Dictionary<string, object?>
            {
                ["Path"] = "/login",
                ["StatusCode"] = 200,
                ["DurationMs"] = 123,
                ["UserId"] = 42
            }
        };

        var result = LogFormatter.Format(entry);

        result.Should().Contain("[green]Path:[/] [white]/login[/]");
        result.Should().Contain("[green]Status:[/] [white]200[/]");
        result.Should().Contain("[green]Duration:[/] [white]123ms[/]");
        result.Should().Contain("UserId");
    }

    [Fact]
    public void Format_ShouldIncludeExceptionDetails()
    {
        var entry = new LogEntry
        {
            Level = "Error",
            Message = "Unhandled exception",
            Exception = new InvalidOperationException("Invalid operation")
        };

        var result = LogFormatter.Format(entry);

        result.Should().Contain("❌");
        result.Should().Contain("Unhandled exception");
        result.Should().Contain("InvalidOperationException");
        result.Should().Contain("Invalid operation");
    }

    [Fact]
    public void Format_ShouldIncludeCorrelationId_WhenPresent()
    {
        var entry = new LogEntry
        {
            Message = "Processed request",
            CorrelationId = "trace-abc-123"
        };

        var result = LogFormatter.Format(entry);

        result.Should().Contain("[cyan]CorrelationId:[/] [white]");
    }
}
