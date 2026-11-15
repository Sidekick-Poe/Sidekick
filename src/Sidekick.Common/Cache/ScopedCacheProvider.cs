using System.Collections.Concurrent;

namespace Sidekick.Common.Cache;

public class ScopedCacheProvider : IScopedCacheProvider
{
    private ConcurrentDictionary<string, object> cache = new();

    /// <inheritdoc />
    public TModel? Get<TModel>(string key)
        where TModel : class
    {
        if (!cache.TryGetValue(key, out var value)) return null;
        return (TModel)value;
    }

    /// <inheritdoc />
    public void Set<TModel>(string key, TModel data)
        where TModel : class
    {
        cache[key] = data;
    }

    /// <inheritdoc />
    public void Clear()
    {
        cache.Clear();
    }

    /// <inheritdoc />
    public void Delete(string key)
    {
        cache.Remove(key, out _);
    }

    /// <inheritdoc />
    public async Task<TModel?> GetOrSet<TModel>(string key, Func<Task<TModel>> func)
        where TModel : class
    {
        var result = Get<TModel>(key);
        if (result != null) return result;

        var data = await func.Invoke();
        Set(key, data);
        return data;
    }
}
