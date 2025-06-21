namespace DotnetObserve.Core.Storage;

/// <summary>
/// General abstraction for storing and retrieving model instances.
/// </summary>
/// <typeparam name="T">The model type to store (e.g., LogEntry).</typeparam>
public interface IStore<T>
{
    Task AppendAsync(T item);

    Task<IReadOnlyList<T>> ReadAllAsync();
}