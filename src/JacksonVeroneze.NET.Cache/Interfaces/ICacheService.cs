using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.Interfaces;

public interface ICacheService
{
    Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default);

    Task<TItem?> GetOrCreateAsync<TItem>(string key,
        Func<CacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key,
        CancellationToken cancellationToken = default);

    Task SetAsync<TItem>(string key,
        TItem value,
        Action<CacheEntryOptions> action,
        CancellationToken cancellationToken = default);

    ICacheService WithPrefixKey(string prefixKey);
}