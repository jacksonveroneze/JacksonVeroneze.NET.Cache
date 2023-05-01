using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace JacksonVeroneze.NET.Cache.DistributedCache.Extensions;

[ExcludeFromCodeCoverage]
public static class SerializeDeserializeExtensions
{
    public static byte[] Serialize<TItem>(this TItem item)
    {
        Models.CacheEntry<TItem> entry = new(item);

        string json = JsonSerializer.Serialize(entry);

        byte[] value = Encoding.UTF8.GetBytes(json);

        return value;
    }

    public static TItem Deserialize<TItem>(this byte[]? value)
    {
        Models.CacheEntry<TItem> item = JsonSerializer
            .Deserialize<Models.CacheEntry<TItem>>(value);

        return item.Value;
    }
}