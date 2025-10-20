using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Hybrid;

namespace Persistence.Repositories.Helper;


public sealed class DefaultHybridCache : HybridCache
{
    private readonly IMemoryCache _memoryCache;

    public DefaultHybridCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public override async ValueTask<T> GetOrCreateAsync<TState, T>(string key, TState state, Func<TState, CancellationToken, ValueTask<T>> factory, HybridCacheEntryOptions? options = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(factory);

        if (_memoryCache.TryGetValue(key, out var boxed) && boxed is T cached && cached is not null)
        {
            return cached;
        }

        var value = await factory(state, cancellationToken).ConfigureAwait(false);

        var memOptions = new MemoryCacheEntryOptions();
        if (options?.Expiration is TimeSpan exp)
        {
            memOptions.AbsoluteExpirationRelativeToNow = exp;
        }

        _memoryCache.Set(key, value, memOptions);

        return value!;
    }

    public override ValueTask SetAsync<T>(string key, T value, HybridCacheEntryOptions? options = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        var memOptions = new MemoryCacheEntryOptions();
        if (options?.Expiration is TimeSpan exp)
        {
            memOptions.AbsoluteExpirationRelativeToNow = exp;
        }

        _memoryCache.Set(key, value!, memOptions);
        return ValueTask.CompletedTask;
    }

    public override ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        _memoryCache.Remove(key);
        return ValueTask.CompletedTask;
    }

    public override ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }
}