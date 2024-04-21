using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.DistributedCache.Adapters;
using Microsoft.Extensions.DependencyInjection;
using MonkeyCache.FileStore;

namespace JacksonVeroneze.NET.DistributedCache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCacheService(
        this IServiceCollection services, string? applicationId)
    {
        ArgumentException.ThrowIfNullOrEmpty(applicationId);

        Barrel.ApplicationId = applicationId;

        services.AddScoped<ICacheAdapter, DistributedCacheAdapter>();
        services.AddCacheService();

        return services;
    }
}