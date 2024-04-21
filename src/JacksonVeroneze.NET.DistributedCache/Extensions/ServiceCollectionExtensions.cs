using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.DistributedCache.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.DistributedCache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCacheService(
        this IServiceCollection services)
    {

        services.AddScoped<ICacheAdapter, DistributedCacheAdapter>();
        services.AddCacheService();

        return services;
    }
}