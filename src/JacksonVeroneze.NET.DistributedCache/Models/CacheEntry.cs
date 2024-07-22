using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace JacksonVeroneze.NET.DistributedCache.Models;

[ExcludeFromCodeCoverage]
[method: JsonConstructor]
public readonly struct CacheEntry<TItem>(TItem value)
{
    public TItem Value { get; } = value;
}