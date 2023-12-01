using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using JacksonVeroneze.NET.DistributedCache.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace JacksonVeroneze.NET.DistributedCache.Adapters;

public class DistributedCacheAdapter(
    IDistributedCache cache) : ICacheAdapter
{
    public async Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default)
    {
        byte[]? value = await cache.GetAsync(
            key, cancellationToken);

        bool existsItem = value is not null && value.Length != 0;

        return existsItem ? value.Deserialize<TItem>() : default;
    }

    public Task RemoveAsync(string key,
        CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key, cancellationToken);
    }

    public Task SetAsync<TItem>(string key,
        TItem value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        DistributedCacheEntryOptions cacheOptions = new()
        {
            AbsoluteExpiration = options.AbsoluteExpiration,
            SlidingExpiration = options.SlidingExpiration,
            AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow
        };

        return cache.SetAsync(key,
            value.Serialize(), cacheOptions, cancellationToken);
    }
}