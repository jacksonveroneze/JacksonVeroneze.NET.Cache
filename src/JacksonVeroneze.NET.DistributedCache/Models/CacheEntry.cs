using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace JacksonVeroneze.NET.DistributedCache.Models;

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