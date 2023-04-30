using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace JacksonVeroneze.NET.Cache.Adapters;

public class DistributedCacheAdapter : ICacheAdapter
{
    private readonly IDistributedCache _cache;

    public DistributedCacheAdapter(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default)
    {
        byte[]? value = await _cache.GetAsync(
            key, cancellationToken);

        bool existsItem = value is not null && value.Length != 0;

        return existsItem ? value.Deserialize<TItem>() : default;
    }

    public Task RemoveAsync(string key,
        CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
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

        return _cache.SetAsync(key,
            value.Serialize(), cacheOptions, cancellationToken);
    }
}