

using System.Text.Json;

namespace DotnetObserve.Core.Storage;

/// <summary>
/// Simple JSON-backed file store for persisting model instances.
/// </summary>
/// <typeparam name="T">The model type to store.</typeparam>
public class JsonFileStore<T> : IStore<T>
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonFileStore(string filePath)
    {
        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task AppendAsync(T item)
    {
        var list = await ReadAllAsync();
        var newList = list.ToList();

        newList.Add(item);

        await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(newList, _jsonOptions));
    }

    public async Task<IReadOnlyList<T>> ReadAllAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<IReadOnlyList<T>>(json, _jsonOptions)
            ?? new List<T>();
    }
}