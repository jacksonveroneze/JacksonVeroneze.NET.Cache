using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using JacksonVeroneze.NET.Cache.Interfaces.Cache;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Service;

public class CacheService : ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly IDistributedCache _cache;

    public CacheService(ILogger<CacheService> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<TItem> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default)
    {
        string value = await _cache.GetStringAsync(key,
            cancellationToken);

        _logger.LogInformation(
            "{class} - {method} - Key: '{key}' - Exists: {exists}",
            nameof(CacheService), nameof(GetAsync), 
            key, value != null);

        return value != null
            ? JsonSerializer.Deserialize<TItem>(value)
            : null;
    }

    public Task SetAsync<TItem>(string key, TItem value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{class} - {method} - Key '{key}'",
            nameof(CacheService), nameof(SetAsync), key);

        return _cache.SetStringAsync(key,
            JsonSerializer.Serialize(value), options,
            cancellationToken);
    }

    public Task RemoveAsync(string key,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "{class} - {method} - Key '{key}'",
            nameof(CacheService), nameof(RemoveAsync), key);

        return _cache.RemoveAsync(key,
            cancellationToken);
    }
}