using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Services;
using JacksonVeroneze.NET.DistributedCache.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.DistributedCache.Extensions;

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