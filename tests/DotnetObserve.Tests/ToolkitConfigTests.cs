using DotnetObserve.Core.Models;
using FluentAssertions;

namespace DotnetObserve.Tests
{
    public class ToolkitConfigTests
    {
        [Fact]
        public void DefaultCtor_HasExpectedDefaults()
        {
            // Arrange
            var config = new ToolkitConfig();

            // Act & Assert
            config.LogLevel.Should().Be("Info");
            config.EnableMetrics.Should().BeTrue();
            config.EnableTracing.Should().BeTrue();
            config.Exporter.Should().Be("File");
        }
    }
}