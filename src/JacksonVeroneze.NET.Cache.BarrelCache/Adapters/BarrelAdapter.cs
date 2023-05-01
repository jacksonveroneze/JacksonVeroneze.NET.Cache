using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using MonkeyCache.FileStore;

namespace JacksonVeroneze.NET.Cache.BarrelCache.Adapters;

public class BarrelAdapter : ICacheAdapter
{
    public BarrelAdapter()
    {
        Barrel.ApplicationId = "application_id";
        Barrel.Current.AutoExpire = true;
    }

    public Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default) where TItem : class
    {
        TItem? result = Barrel.Current.Get<TItem>(key);

        return Task.FromResult(result)!;
    }

    public Task RemoveAsync(string key,
        CancellationToken cancellationToken = default)
    {
        Barrel.Current.Empty(key);

        return Task.CompletedTask;
    }

    public Task SetAsync<TItem>(string key,
        TItem value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default) where TItem : class
    {
        Barrel.Current.Add(key, value,
            expireIn: options.AbsoluteExpirationRelativeToNow!.Value);

        return Task.CompletedTask;
    }
}