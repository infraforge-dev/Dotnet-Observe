using System.Text.Json;
using DotnetObserve.Core.Models;
using FluentAssertions;

namespace DotnetObserve.Tests;

public class TraceSpanTests
{
    [Fact]
    public void DefaultCtor_SetsIdAndTimestamp()
    {
        // Arrange & Act
        var span = new TraceSpan();
        span.Id.Should().NotBeEmpty();
        span.TraceId.Should().NotBeNullOrEmpty();
        span.SpanId.Should().NotBeNullOrEmpty();
        span.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        span.EndTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        span.Name.Should().BeEmpty();
        span.ParentSpanId.Should().BeNull();
        span.Attributes.Should().BeNull();
    }

    [Fact]
    public void CanSerializeAndDeserialize_ToJson()
    {
        // Arrange
        var span = new TraceSpan
        {
            Name = "TestSpan",
            ParentSpanId = "parent-123",
            Attributes = new Dictionary<string, object>
            {
                { "Key1", "Value1" },
                { "Key2", 100 }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(span);
        var roundTripSpan = JsonSerializer.Deserialize<TraceSpan>(json);

        // Assert
        roundTripSpan.Should().NotBeNull();
        roundTripSpan.Id.Should().Be(span.Id);
        roundTripSpan.TraceId.Should().Be(span.TraceId);
        roundTripSpan.SpanId.Should().Be(span.SpanId);
        roundTripSpan.Name.Should().Be(span.Name);
        roundTripSpan.ParentSpanId.Should().Be(span.ParentSpanId);
        roundTripSpan.Attributes!["Key1"].Should().BeOfType<JsonElement>();
        ((JsonElement)roundTripSpan.Attributes!["Key1"]).GetString().Should().Be("Value1");
        roundTripSpan.Attributes!["Key2"].Should().BeOfType<JsonElement>();
        ((JsonElement)roundTripSpan.Attributes!["Key2"]).GetInt32().Should().Be(100);
    }
}