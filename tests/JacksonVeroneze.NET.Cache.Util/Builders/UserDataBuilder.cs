using System.Text;
using System.Text.Json;
using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.Util.Builders;

[ExcludeFromCodeCoverage]
public class UserDataBuilder
{
    private UserDataBuilder()
    {
    }

    public static byte[] BuildSingle<TType>(TType user)
    {
        CacheEntry<TType> entry = new(user);

        string json = JsonSerializer.Serialize(entry);

        byte[] value = Encoding.UTF8.GetBytes(json);

        return value;
    }
}