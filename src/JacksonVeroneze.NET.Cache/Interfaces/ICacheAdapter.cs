using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.Interfaces;

public interface ICacheAdapter
{
    public Task<TItem?> GetAsync<TItem>(
        string key,
        CancellationToken cancellationToken = default);

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);

    public Task SetAsync<TItem>(
        string key,
        TItem value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default);
}