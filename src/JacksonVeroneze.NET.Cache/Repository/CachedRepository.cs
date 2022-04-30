using System;
using System.Threading;
using System.Threading.Tasks;

using JacksonVeroneze.NET.Cache.Interfaces.Cache;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Repository;

public class CachedRepository
{
    private readonly ILogger<CachedRepository> _logger;
    private readonly ICacheService _cache;

    public CachedRepository(
        ILogger<CachedRepository> logger,
        ICacheService cache)
    {
        _logger = logger;
        _cache = cache;
    }

    protected async Task CreateAsync<TItem>(string key, TItem item,
        Action<DistributedCacheEntryOptions> action,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(null, nameof(key));

        if (action is null)
            throw new ArgumentException(null, nameof(action));

        DistributedCacheEntryOptions options = new ();

        action?.Invoke(options);

        await _cache.SetAsync(key, item, options, cancellationToken);

        _logger.LogInformation("{class} - {method} - Added '{key}'",
            nameof(CachedRepository), nameof(CreateAsync), key);
    }

    protected async Task<TItem> GetOrCreateAsync<TItem>(string key,
        Func<DistributedCacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(null, nameof(key));

        if (factory is null)
            throw new ArgumentException(null, nameof(factory));

        TItem item = await _cache.GetAsync<TItem>(key, cancellationToken);

        if (item != null)
        {
            _logger.LogInformation("{class} - {method} - UseCache '{key}'",
                nameof(CachedRepository), nameof(GetOrCreateAsync), key);

            return item;
        }

        DistributedCacheEntryOptions options = new();

        TItem result = await factory.Invoke(options);

        await _cache.SetAsync(key, result, options, cancellationToken);

        _logger.LogInformation("{class} - {method} - UseSource '{key}'",
            nameof(CachedRepository), nameof(GetOrCreateAsync), key);

        return result;
    }

    protected Task RemoveAsync(string key,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("{class} - {method} - Remove Key: '{key}'",
            nameof(CachedRepository), nameof(RemoveAsync), key);

        return _cache.RemoveAsync(key, cancellationToken);
    }
}
