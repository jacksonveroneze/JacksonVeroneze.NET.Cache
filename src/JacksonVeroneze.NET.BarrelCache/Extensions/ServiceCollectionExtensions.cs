using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.BarrelCache.Adapters;
using JacksonVeroneze.NET.Cache.Extensions;
using JacksonVeroneze.NET.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MonkeyCache.FileStore;

namespace JacksonVeroneze.NET.BarrelCache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBarrelCacheService(
        this IServiceCollection services,
        string? applicationId,
        bool autoExpire = true)
    {
        ArgumentException.ThrowIfNullOrEmpty(applicationId);

        Barrel.ApplicationId = applicationId;
        Barrel.Current.AutoExpire = autoExpire;

        services.AddScoped<ICacheAdapter, BarrelCacheAdapter>();
        services.AddCacheService();

        return services;
    }
}