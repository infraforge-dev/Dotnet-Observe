using System.Text.Json;
using DotnetObserve.Core.Models;
using FluentAssertions;

namespace DotnetObserve.Tests;

public class LogEntryTests
{
    [Fact]
    public void DefaultCtor_SetsIdAndTimestamp()
    {
        // Arrange & Act
        var entry = new LogEntry();
        entry.Id.Should().NotBeEmpty();
        entry.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entry.Level.Should().Be("Info");
        entry.Context.Should().BeEmpty();
    }

    [Fact]
    public void CanSerializeAndDeserialize_ToJson()
    {
        // Arrange
        var entry = new LogEntry
        {
            Message = "Test log message",
            Source = "UnitTest",
            CorrelationId = "corr-123",
            Context = new Dictionary<string, object?>
            {
                { "UserId", 42 },
                { "Operation", "TestOp" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(entry);
        var roundTripEntry = JsonSerializer.Deserialize<LogEntry>(json);

        // Assert
        roundTripEntry.Should().NotBeNull();
        roundTripEntry.Message.Should().Be(entry.Message);
        roundTripEntry.Source.Should().Be(entry.Source);
        roundTripEntry.CorrelationId.Should().Be(entry.CorrelationId);
        roundTripEntry.Context!["UserId"].Should().BeOfType<JsonElement>();
        roundTripEntry.Context!["UserId"].Should().BeOfType<JsonElement>().Which.GetInt32().Should().Be(42);
        roundTripEntry.Context!["Operation"].Should().BeOfType<JsonElement>();
        roundTripEntry.Context!["Operation"].Should().BeOfType<JsonElement>().Which.GetString().Should().Be("TestOp");
    }
}