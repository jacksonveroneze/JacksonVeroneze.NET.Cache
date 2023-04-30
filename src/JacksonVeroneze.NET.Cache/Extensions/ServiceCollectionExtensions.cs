using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.Cache.Adapters;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.Cache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistribCache(
        this IServiceCollection services)
    {
        services.AddTransient<ICacheAdapter, DistributedCacheAdapter>();
        services.AddTransient<ICacheService, CacheService>();

        return services;
    }
}