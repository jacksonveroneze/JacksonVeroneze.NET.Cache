using System.Diagnostics.CodeAnalysis;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.NET.Cache.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCacheService(
        this IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}