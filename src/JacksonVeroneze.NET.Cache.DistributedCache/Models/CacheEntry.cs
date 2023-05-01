using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace JacksonVeroneze.NET.Cache.DistributedCache.Models;

[ExcludeFromCodeCoverage]
public readonly struct CacheEntry<TItem>
{
    public TItem Value { get; }

    [JsonConstructor]
    public CacheEntry(TItem value)
    {
        Value = value;
    }
}