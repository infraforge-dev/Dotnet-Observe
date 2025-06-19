using DotnetObserve.Core.Abstractions;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;

namespace DotnetObserve.Core.Logging;

/// <summary>
/// Default implementation of <see cref="IObservabilityLogger"/> that writes log entries to an <see cref="IStore{LogEntry}"/>.
/// </summary>
public class DefaultObservabilityLogger : IObservabilityLogger
{
    private readonly IStore<LogEntry> _store;

    public DefaultObservabilityLogger(IStore<LogEntry> store)
    {
        _store = store;
    }

    public async Task LogAsync(LogEntry entry)
    {
        await _store.AppendAsync(entry);
    }
}
