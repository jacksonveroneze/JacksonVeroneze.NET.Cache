using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

namespace JacksonVeroneze.NET.Cache.Interfaces.Cache;

public interface ICacheService
{
    Task<TItem> GetAsync<TItem>(string key,
        CancellationToken cancellationToken = default);

    Task SetAsync<TItem>(string key, TItem value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key,
        CancellationToken cancellationToken = default);
}
