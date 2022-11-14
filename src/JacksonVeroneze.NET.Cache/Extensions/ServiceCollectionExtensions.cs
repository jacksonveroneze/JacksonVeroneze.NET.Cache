using JacksonVeroneze.NET.Cache.Interfaces.Cache;
using JacksonVeroneze.NET.Cache.Service;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.Cache.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistribCache(
        this IServiceCollection services)
    {
        services.AddTransient<ICacheService, CacheService>();

        return services;
    }
}