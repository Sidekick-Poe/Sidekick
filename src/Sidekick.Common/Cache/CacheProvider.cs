using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Cache
{
    /// <summary>
    /// Implementation for the cache provider.
    /// </summary>
    public class CacheProvider : ICacheProvider
    {
        private const string cachePath = "SidekickCache";
        private readonly ILogger<CacheProvider> logger;

        public CacheProvider(ILogger<CacheProvider> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TModel?> Get<TModel>(string key)
            where TModel : class
        {
            EnsureDirectory();

            var fileName = GetCacheFileName(key);

            if (!File.Exists(fileName))
            {
                return null;
            }

            using var stream = File.OpenRead(fileName);
            try
            {
                return await JsonSerializer.DeserializeAsync<TModel>(stream);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc/>
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

                using var stream = File.Create(fileName);
                await JsonSerializer.SerializeAsync(stream, data);
            }
            catch (IOException exception)
            {
                logger.LogError($"[Cache] Failed to set cache for key {key}.", exception);
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            EnsureDirectory();

            Directory.Delete(Path.Combine(SidekickPaths.GetDataFilePath(), cachePath), true);
        }

        /// <inheritdoc/>
        public void Delete(string key)
        {
            EnsureDirectory();

            var fileName = GetCacheFileName(key);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        /// <inheritdoc/>
        public async Task<TModel> GetOrSet<TModel>(string key, Func<Task<TModel>> func) where TModel : class
        {
            EnsureDirectory();

            var result = await Get<TModel>(key);

            if (result == null)
            {
                var data = await func.Invoke();
                await Set(key, data);
                return data;
            }

            return result;
        }

        private static void EnsureDirectory()
        {
            Directory.CreateDirectory(Path.Combine(SidekickPaths.GetDataFilePath(), cachePath));
        }

        private static string GetCacheFileName(string key)
        {
            key = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
            key += ".json";

            return Path.Combine(SidekickPaths.GetDataFilePath(), cachePath, key);
        }
    }
}
