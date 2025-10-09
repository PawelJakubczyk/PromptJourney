using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Tests.TestInfrastructure;

public static class TestCacheExtensions
{
    public static IServiceCollection AddHybridCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<HybridCache>();
        return services;
    }
}

public class HybridCache
{
    private readonly IMemoryCache _memoryCache;

    public HybridCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key) where T : class
    {
        return _memoryCache.Get<T>(key);
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        _memoryCache.Set(key, value, options);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}