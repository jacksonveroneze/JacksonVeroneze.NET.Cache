using System.Text.Json.Serialization;

namespace JacksonVeroneze.NET.Cache.Models;

internal readonly struct CacheEntry<TItem>
{
    public TItem Value { get; }

    [JsonConstructor]
    public CacheEntry(TItem value)
    {
        Value = value;
    }
}