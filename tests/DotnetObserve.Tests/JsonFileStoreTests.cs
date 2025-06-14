using DotnetObserve.Core.Storage;
using FluentAssertions;

namespace DotnetObserve.Tests;

public class JsonFileStoreTests
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "test_store.json");

    [Fact]
    public async Task AppendAndReadAll_worksCorrectly()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }

        var store = new JsonFileStore<int>(_testFilePath);

        await store.AppendAsync(1);
        await store.AppendAsync(2);

        var all = await store.ReadAllAsync();

        all.Should().BeEquivalentTo(new[] { 1, 2 });

        File.Delete(_testFilePath);
    }
}