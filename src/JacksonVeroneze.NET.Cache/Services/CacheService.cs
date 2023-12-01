using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Services;

public class CacheService(
    ILogger<CacheService> logger,
    ICacheAdapter adapter) : ICacheService
{
    private string _prefixKey = string.Empty;

    public ICacheService WithPrefixKey(string prefixKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefixKey);

        _prefixKey = prefixKey;

        return this;
    }

    public async Task<TItem?> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);

        string formatedKey = FormatKey(key);

        TItem? value = await adapter.GetAsync<TItem?>(
            formatedKey, cancellationToken);

        logger.LogGet(nameof(CacheService),
            nameof(GetAsync),
            formatedKey,
            value != null);

        return value;
    }

    public async Task<TItem?> GetOrCreateAsync<TItem>(string key,
        Func<CacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        string formatedKey = FormatKey(key);

        TItem? value = await adapter.GetAsync<TItem?>(
            formatedKey, cancellationToken);

        if (value is not null)
        {
            logger.LogGet(nameof(CacheService),
                nameof(GetOrCreateAsync),
                formatedKey,
                true);

            return value;
        }

        CacheEntryOptions options = new();

        TItem item = await factory.Invoke(options);

        if (item == null && !options.AllowStoreNullValue)
        {
            return item;
        }

        await adapter.SetAsync(formatedKey,
            item, options, cancellationToken);

        logger.LogSet(nameof(CacheService),
            nameof(GetOrCreateAsync),
            formatedKey);

        return item;
    }

    public async Task RemoveAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);

        string formatedKey = FormatKey(key);

        await adapter.RemoveAsync(formatedKey,
            cancellationToken);

        logger.LogRemove(nameof(CacheService),
            nameof(RemoveAsync),
            formatedKey);
    }

    public async Task SetAsync<TItem>(string key,
        TItem item,
        Action<CacheEntryOptions> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(action);

        string formatedKey = FormatKey(key);

        CacheEntryOptions options = new();

        action(options);

        if (!(options.AbsoluteExpiration != null ||
              options.AbsoluteExpirationRelativeToNow != null ||
              options.SlidingExpiration != null))
        {
            throw new ArgumentException(
                $"{nameof(CacheEntryOptions)} invalid expiration");
        }

        await adapter.SetAsync(formatedKey,
            item, options, cancellationToken);

        logger.LogSet(nameof(CacheService),
            nameof(SetAsync),
            formatedKey);
    }

    #region Key

    private string FormatKey(string key)
    {
        return string.Concat(_prefixKey, key);
    }

    #endregion
}