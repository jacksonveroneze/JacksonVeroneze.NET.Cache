using System.Text;
using System.Text.Json;
using JacksonVeroneze.NET.DistributedCache.Models;

namespace JacksonVeroneze.NET.Cache.Util.Builders;

[ExcludeFromCodeCoverage]
public static class UserDataBuilder
{
    public static byte[] BuildSingle<TType>(TType user)
    {
        CacheEntry<TType> entry = new(user);

        string json = JsonSerializer.Serialize(entry);

        byte[] value = Encoding.UTF8.GetBytes(json);

        return value;
    }
}