using Microsoft.Extensions.Caching.Distributed;

namespace JacksonVeroneze.NET.Cache.Interfaces;

public interface ICacheService
{
    Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default);

    Task<TItem> GetOrCreateAsync<TItem>(string key,
        Func<DistributedCacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key,
        CancellationToken cancellationToken = default);

    Task SetAsync<TItem>(string key, TItem value,
        Action<DistributedCacheEntryOptions> action,
        CancellationToken cancellationToken = default);

    ICacheService WithPrefixKey(string prefixKey);
}