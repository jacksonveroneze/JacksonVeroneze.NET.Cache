using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.Cache.DistributedCache.Adapters;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.Cache.DistributedCache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCacheService(
        this IServiceCollection services)
    {
        services.AddTransient<ICacheAdapter, DistributedCacheAdapter>();
        services.AddTransient<ICacheService, CacheService>();

        return services;
    }
}