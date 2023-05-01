using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.Interfaces;

public interface ICacheAdapter
{
    Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default) where TItem : class;

    Task RemoveAsync(string key,
        CancellationToken cancellationToken = default);

    Task SetAsync<TItem>(string key,
        TItem value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default) where TItem : class;
}