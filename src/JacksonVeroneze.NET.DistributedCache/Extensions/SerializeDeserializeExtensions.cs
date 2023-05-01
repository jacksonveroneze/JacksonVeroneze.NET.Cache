using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using JacksonVeroneze.NET.DistributedCache.Models;

namespace JacksonVeroneze.NET.DistributedCache.Extensions;

[ExcludeFromCodeCoverage]
public static class SerializeDeserializeExtensions
{
    public static byte[] Serialize<TItem>(this TItem item)
    {
        CacheEntry<TItem> entry = new(item);

        string json = JsonSerializer.Serialize(entry);

        byte[] value = Encoding.UTF8.GetBytes(json);

        return value;
    }

    public static TItem Deserialize<TItem>(this byte[]? value)
    {
        CacheEntry<TItem> item = JsonSerializer
            .Deserialize<CacheEntry<TItem>>(value);

        return item.Value;
    }
}