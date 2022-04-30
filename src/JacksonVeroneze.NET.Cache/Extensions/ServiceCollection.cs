using JacksonVeroneze.NET.Cache.Interfaces.Cache;
using JacksonVeroneze.NET.Cache.Service;

using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.Cache.Extensions;

public static class ServiceCollection
{
    public static IServiceCollection AddCache(
        this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}