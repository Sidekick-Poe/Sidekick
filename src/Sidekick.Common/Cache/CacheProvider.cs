using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Cache;

/// <summary>
///     Implementation for the cache provider.
/// </summary>
public class CacheProvider(ILogger<CacheProvider> logger) : ICacheProvider
{
    private const string CachePath = "SidekickCache";

    /// <inheritdoc />
    public async Task<TModel?> Get<TModel>(string key, Func<TModel, bool> cacheValidator)
        where TModel : class
    {
        EnsureDirectory();

        var fileName = GetCacheFileName(key);

        if (!File.Exists(fileName))
        {
            return null;
        }

        await using var stream = File.OpenRead(fileName);
        try
        {
            var value = await JsonSerializer.DeserializeAsync<TModel>(stream);
            if (value == null) return null;

            var valid = cacheValidator.Invoke(value);
            if (!valid) return null;

            return value;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task Set<TModel>(string key, TModel data)
        where TModel : class
    {
        try
        {
            EnsureDirectory();

            var fileName = GetCacheFileName(key);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            await using var stream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(stream, data);
        }
        catch (IOException exception)
        {
            logger.LogError(message: $"[Cache] Failed to set cache for key {key}.", exception);
        }
    }

    /// <inheritdoc />
    public async Task Clear()
    {
        EnsureDirectory();
        Directory.Delete(path: Path.Combine(path1: SidekickPaths.GetDataFilePath(), CachePath), true);
        await Task.Delay(100);
    }

    /// <inheritdoc />
    public void Delete(string key)
    {
        EnsureDirectory();

        var fileName = GetCacheFileName(key);
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }

    /// <inheritdoc />
    public async Task<TModel> GetOrSet<TModel>(string key, Func<Task<TModel>> func, Func<TModel, bool> cacheValidator)
        where TModel : class
    {
        EnsureDirectory();

        var result = await Get(key, cacheValidator);

        if (result != null)
        {
            return result;
        }

        var data = await func.Invoke();
        await Set(key, data);
        return data;
    }

    private static void EnsureDirectory()
    {
        Directory.CreateDirectory(Path.Combine(path1: SidekickPaths.GetDataFilePath(), CachePath));
    }

    private static string GetCacheFileName(string key)
    {
        key = string.Join("_", value: key.Split(Path.GetInvalidFileNameChars()));
        key += ".json";

        return Path.Combine(path1: SidekickPaths.GetDataFilePath(), CachePath, key);
    }
}
