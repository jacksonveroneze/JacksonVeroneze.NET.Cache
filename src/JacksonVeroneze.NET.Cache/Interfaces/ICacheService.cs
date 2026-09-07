using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.Interfaces;

public interface ICacheService
{
    public ICacheService SetPrefixKey(
        string prefixKey);

    public Task<TItem?> TryGetAsync<TItem>(
        string key,
        CancellationToken cancellationToken = default);

    public Task<TItem?> TryGetOrCreateAsync<TItem>(
        string key,
        Func<CacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default);

    public Task<bool> TryRemoveAsync(
        string key,
        CancellationToken cancellationToken = default);

    public Task<bool> TrySetAsync<TItem>(
        string key,
        TItem value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default);
}