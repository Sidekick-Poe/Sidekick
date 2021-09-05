using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sidekick.Common.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private const string cachePath = "SidekickCache";

        public async Task<TModel> Get<TModel>(string key)
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

        public async Task Set<TModel>(string key, TModel data)
            where TModel : class
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

        public void Clear()
        {
            Directory.Delete(Path.Combine(SidekickPaths.GetDataFilePath(), cachePath), true);
        }

        public void Delete(string key)
        {
            var fileName = GetCacheFileName(key);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

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
