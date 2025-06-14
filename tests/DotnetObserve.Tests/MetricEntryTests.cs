using System.Text.Json;
using DotnetObserve.Core.Models;
using FluentAssertions;

namespace DotnetObserve.Tests;

public class MetricEntryTests
{
    [Fact]
    public void DefaultCtor_SetsIdAndTimestamp()
    {
        // Arrange & Act
        var entry = new MetricEntry();
        entry.Id.Should().NotBeEmpty();
        entry.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entry.Name.Should().BeEmpty();
        entry.Value.Should().Be(0);
    }

    [Fact]
    public void CanSerializeAndDeserialize_ToJson()
    {
        // Arrange
        var entry = new MetricEntry
        {
            Name = "Test Metric",
            Value = 123.45,
            Unit = "ms",
            Tags = "endpoint:/api/test"
        };

        // Act
        var json = JsonSerializer.Serialize(entry);
        var roundTripEntry = JsonSerializer.Deserialize<MetricEntry>(json);

        // Assert
        roundTripEntry.Should().NotBeNull();
        roundTripEntry.Name.Should().Be(entry.Name);
        roundTripEntry.Value.Should().Be(entry.Value);
        roundTripEntry.Unit.Should().Be(entry.Unit);
        roundTripEntry.Tags.Should().Be(entry.Tags);
    }
}