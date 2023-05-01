using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.Cache.BarrelCache.Adapters;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.Cache.BarrelCache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBarrelCacheService(
        this IServiceCollection services)
    {
        services.AddTransient<ICacheAdapter, BarrelAdapter>();
        services.AddTransient<ICacheService, CacheService>();

        return services;
    }
}