using System.Text;
using System.Text.Json;
using JacksonVeroneze.NET.Cache.Interfaces.Cache;
using JacksonVeroneze.NET.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Service;

public class CacheService : ICacheService
{
    private string _prefixKey;
    private readonly ILogger<CacheService> _logger;
    private readonly IDistributedCache _cache;

    public CacheService(ILogger<CacheService> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;

        _prefixKey = "_default_";
    }

    public ICacheService WithPrefixKey(string prefixKey)
    {
        ArgumentNullException.ThrowIfNull(prefixKey, nameof(prefixKey));

        _prefixKey = prefixKey;

        return this;
    }

    public async Task<TItem> GetOrCreateAsync<TItem>(string key,
        Func<DistributedCacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_prefixKey, nameof(_prefixKey));
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));

        string formatedKey = FormatKey(key);

        byte[] value = await _cache.GetAsync(formatedKey,
            cancellationToken);

        bool existsItem = value is not null && value.Length != 0;

        if (existsItem)
        {
            _logger.LogDebug(
                "{class} - {method} - Key: '{key}' - InCache",
                nameof(CacheService),
                nameof(GetOrCreateAsync),
                formatedKey);

            return Deserialize<TItem>(value);
        }

        DistributedCacheEntryOptions options = new();

        TItem item = await factory.Invoke(options);

        await _cache.SetAsync(formatedKey,
            Serialize(item), options, cancellationToken);

        _logger.LogDebug(
            "{class} - {method} - Key: '{key}' - Added InCache",
            nameof(CacheService),
            nameof(GetOrCreateAsync),
            formatedKey);

        return item;
    }

    public async Task<TItem> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_prefixKey, nameof(_prefixKey));
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        string formatedKey = FormatKey(key);

        byte[] value = await _cache.GetAsync(formatedKey,
            cancellationToken);

        bool existsItem = value is null || value.Length == 0;

        _logger.LogDebug(
            "{class} - {method} - Key: '{key}' - Exists: {exists}",
            nameof(CacheService),
            nameof(GetAsync),
            formatedKey,
            existsItem);

        return existsItem ? Deserialize<TItem>(value) : default;
    }

    public Task RemoveAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_prefixKey, nameof(_prefixKey));
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        string formatedKey = FormatKey(key);

        _logger.LogDebug(
            "{class} - {method} - Key '{key}'",
            nameof(CacheService),
            nameof(RemoveAsync),
            formatedKey);

        return _cache.RemoveAsync(formatedKey,
            cancellationToken);
    }

    public Task SetAsync<TItem>(string key, TItem item,
        Action<DistributedCacheEntryOptions> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_prefixKey, nameof(_prefixKey));
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        string formatedKey = FormatKey(key);

        _logger.LogDebug("{class} - {method} - Key '{key}'",
            nameof(CacheService),
            nameof(SetAsync),
            formatedKey);

        DistributedCacheEntryOptions options = new();

        action?.Invoke(options);

        return _cache.SetAsync(formatedKey,
            Serialize(item), options, cancellationToken);
    }

    #region Serialize

    private static byte[] Serialize<TItem>(TItem item)
    {
        CacheEntry<TItem> entry = new(item);

        string json = JsonSerializer.Serialize(entry);

        byte[] value = Encoding.UTF8.GetBytes(json);

        return value;
    }

    private static TItem Deserialize<TItem>(byte[] value)
    {
        CacheEntry<TItem> item = JsonSerializer
            .Deserialize<CacheEntry<TItem>>(value);

        return item.Value;
    }

    #endregion

    #region Key

    private string FormatKey(string key)
    {
        return string.Concat(_prefixKey, key);
    }

    #endregion
}