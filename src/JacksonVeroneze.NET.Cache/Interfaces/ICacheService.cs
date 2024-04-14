using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.Interfaces;

public interface ICacheService
{
    ICacheService WithPrefixKey(string prefixKey);

    Task<TItem?> TryGetAsync<TItem>(string key,
        CancellationToken cancellationToken = default);

    Task<TItem?> TryGetOrCreateAsync<TItem>(string key,
        Func<CacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default);

    Task<bool> TryRemoveAsync(string key,
        CancellationToken cancellationToken = default);

    Task<bool> TrySetAsync<TItem>(string key,
        TItem value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default);
}