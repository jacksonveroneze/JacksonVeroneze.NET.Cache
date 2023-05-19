using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Services;

public class CacheService : ICacheService
{
    private string _prefixKey;
    private readonly ILogger<CacheService> _logger;
    private readonly ICacheAdapter _adapter;

    public CacheService(ILogger<CacheService> logger,
        ICacheAdapter adapter)
    {
        _logger = logger;
        _adapter = adapter;

        _prefixKey = string.Empty;
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

        TItem? value = await _adapter.GetAsync<TItem?>(
            formatedKey, cancellationToken);

        _logger.LogGet(nameof(CacheService),
            nameof(GetAsync),
            formatedKey,
            value != null);

        return value;
    }

    public async Task<TItem?> GetOrCreateAsync<TItem>(string key,
        Func<CacheEntryOptions, Task<TItem>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey, nameof(_prefixKey));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));

        string formatedKey = FormatKey(key);

        TItem? value = await _adapter.GetAsync<TItem?>(
            formatedKey, cancellationToken);

        if (value is not null)
        {
            _logger.LogGet(nameof(CacheService),
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

        await _adapter.SetAsync(formatedKey,
            item, options, cancellationToken);

        _logger.LogSet(nameof(CacheService),
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

        await _adapter.RemoveAsync(formatedKey,
            cancellationToken);

        _logger.LogRemove(nameof(CacheService),
            nameof(RemoveAsync),
            formatedKey);
    }

    public async Task SetAsync<TItem>(string key,
        TItem item,
        Action<CacheEntryOptions> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_prefixKey, nameof(_prefixKey));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        ArgumentNullException.ThrowIfNull(action, nameof(action));

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

        await _adapter.SetAsync(formatedKey,
            item, options, cancellationToken);

        _logger.LogSet(nameof(CacheService),
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