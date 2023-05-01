using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.Interfaces;

public interface ICacheService
{
    Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default) where TItem : class;

    Task<TItem?> GetOrCreateAsync<TItem>(string key,
        Func<CacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default) where TItem : class;

    Task RemoveAsync(string key,
        CancellationToken cancellationToken = default);

    Task SetAsync<TItem>(string key,
        TItem value,
        Action<CacheEntryOptions> action,
        CancellationToken cancellationToken = default) where TItem : class;

    ICacheService WithPrefixKey(string prefixKey);
}