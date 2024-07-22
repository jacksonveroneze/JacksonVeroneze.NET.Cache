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

    public ICacheService SetPrefixKey(
        string? prefixKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefixKey);

        _prefixKey = prefixKey;

        return this;
    }

    #region Get

    public async Task<TItem?> TryGetAsync<TItem>(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);

        string formatedKey = FormatKey(key);

        try
        {
            TItem? value = await adapter.GetAsync<TItem?>(
                formatedKey, cancellationToken);

            logger.LogGet(nameof(CacheService),
                nameof(TryGetAsync), formatedKey, value != null);

            return value;
        }
        catch (Exception ex)
        {
            logger.LogGenericError(nameof(CacheService),
                nameof(TryGetAsync), formatedKey, ex);

            return default;
        }
    }

    #endregion

    #region GetOrCreate

    public async Task<TItem?> TryGetOrCreateAsync<TItem>(
        string key,
        Func<CacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        string formatedKey = FormatKey(key);

        TItem? value = await TryGetAsync<TItem?>(
            formatedKey, cancellationToken);

        if (value is not null)
        {
            return value;
        }

        CacheEntryOptions options = new();

        TItem item = await factory.Invoke(options);

        if (item == null && options.AllowStoreNullValue)
        {
            await TrySetAsync(formatedKey,
                item, options, cancellationToken);
        }

        return item;
    }

    #endregion

    #region Remove

    public async Task<bool> TryRemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);

        string formatedKey = FormatKey(key);

        try
        {
            await adapter.RemoveAsync(formatedKey,
                cancellationToken);

            logger.LogRemove(nameof(CacheService),
                nameof(TryRemoveAsync),
                formatedKey);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogGenericError(nameof(CacheService),
                nameof(TryRemoveAsync), formatedKey, ex);

            return false;
        }
    }

    #endregion

    #region Set

    public async Task<bool> TrySetAsync<TItem>(
        string key,
        TItem value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(options);

        string formatedKey = FormatKey(key);

        if (options is
            {
                AbsoluteExpiration: null,
                AbsoluteExpirationRelativeToNow: null,
                SlidingExpiration: null
            })
        {
            throw new ArgumentException(
                $"{nameof(CacheEntryOptions)} invalid expiration");
        }

        try
        {
            await adapter.SetAsync(formatedKey,
                value, options, cancellationToken);

            logger.LogSet(nameof(CacheService),
                nameof(TrySetAsync),
                formatedKey);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogGenericError(nameof(CacheService),
                nameof(TrySetAsync), formatedKey, ex);

            return false;
        }
    }

    #endregion

    #region Key

    private string FormatKey(string key)
    {
        return string.Concat(_prefixKey, key);
    }

    #endregion
}