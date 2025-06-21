using DotnetObserve.Core.Logging;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using FluentAssertions;

namespace DotnetObserve.Tests;

public class DefaultObservabilityLoggerTests
{
    private class InMemoryStore<T> : IStore<T>
    {
        public List<T> Items { get; } = new();

        public Task AppendAsync(T item)
        {
            Items.Add(item);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<T>> ReadAllAsync()
        {
            return Task.FromResult((IReadOnlyList<T>)Items);
        }
    }

    [Fact]
    public async Task LogAsync_AppendsLogEntryToStore()
    {
        // Arrange
        var store = new InMemoryStore<LogEntry>();
        var logger = new DefaultObservabilityLogger(store);

        var entry = new LogEntry
        {
            Level = "Info",
            Message = "Test log entry"
        };

        // Act
        await logger.LogAsync(entry);

        // Assert
        store.Items.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(entry);
    }
}