using System.Text;
using System.Text.Json;
using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Services;

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
        ArgumentException.ThrowIfNullOrEmpty(prefixKey, nameof(prefixKey));

        _prefixKey = prefixKey;

        return this;
    }

    public async Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey, nameof(_prefixKey));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        string formatedKey = FormatKey(key);

        byte[]? value = await _cache.GetAsync(formatedKey,
            cancellationToken);

        bool existsItem = value is not null && value.Length != 0;

        _logger.LogGet(nameof(CacheService),
            nameof(GetAsync),
            formatedKey,
            existsItem);

        return existsItem ? Deserialize<TItem>(value) : default;
    }

    public async Task<TItem?> GetOrCreateAsync<TItem>(string key,
        Func<DistributedCacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey, nameof(_prefixKey));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));

        string formatedKey = FormatKey(key);

        byte[]? value = await _cache.GetAsync(formatedKey,
            cancellationToken);

        bool existsItem = value is not null && value.Length != 0;

        if (existsItem)
        {
            _logger.LogGetOrCreateInCache(nameof(CacheService),
                nameof(GetOrCreateAsync),
                formatedKey);

            return Deserialize<TItem>(value);
        }

        DistributedCacheEntryOptions options = new();

        TItem item = await factory.Invoke(options);

        await _cache.SetAsync(formatedKey,
            Serialize(item), options, cancellationToken);

        _logger.LogGetOrCreateNotInCache(nameof(CacheService),
            nameof(GetOrCreateAsync),
            formatedKey);

        return item;
    }

    public async Task RemoveAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey, nameof(_prefixKey));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        string formatedKey = FormatKey(key);

        await _cache.RemoveAsync(formatedKey,
            cancellationToken);

        _logger.LogRemove(nameof(CacheService),
            nameof(RemoveAsync),
            formatedKey);
    }

    public async Task SetAsync<TItem>(string key, TItem item,
        Action<DistributedCacheEntryOptions> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey, nameof(_prefixKey));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        string formatedKey = FormatKey(key);

        DistributedCacheEntryOptions options = new();

        action(options);

        await _cache.SetAsync(formatedKey,
            Serialize(item), options, cancellationToken);

        _logger.LogSet(nameof(CacheService),
            nameof(SetAsync),
            formatedKey);
    }

    #region Serialize

    private static byte[] Serialize<TItem>(TItem item)
    {
        CacheEntry<TItem> entry = new(item);

        string json = JsonSerializer.Serialize(entry);

        byte[] value = Encoding.UTF8.GetBytes(json);

        return value;
    }

    private static TItem Deserialize<TItem>(byte[]? value)
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