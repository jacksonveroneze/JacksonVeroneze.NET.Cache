using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.BarrelCache.Adapters;
using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.BarrelCache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBarrelCacheService(
        this IServiceCollection services)
    {
        services.AddScoped<ICacheAdapter, BarrelCacheAdapter>();
        services.AddCacheService();

        return services;
    }
}