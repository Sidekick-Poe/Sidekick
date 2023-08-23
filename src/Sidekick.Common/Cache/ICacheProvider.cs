namespace Sidekick.Common.Cache
{
    /// <summary>
    /// Interface for the cache provider.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Gets the value of the cache if it is set. Otherwise returns null.
        /// </summary>
        /// <typeparam name="TModel">THe type of the data</typeparam>
        /// <param name="key">The key under which to save the cache</param>
        /// <param name="func">The func to initialize the data, in the event that the data is not in the cache</param>
        /// <returns>Returns the cache data</returns>
        Task<TModel?> Get<TModel>(string key)
            where TModel : class;

        /// <summary>
        /// Sets the value of the cache.
        /// </summary>
        /// <typeparam name="TModel">THe type of the data</typeparam>
        /// <param name="key">The key under which to save the cache</param>
        /// <param name="data">The data to save in the cache</param>
        Task Set<TModel>(string key, TModel data)
            where TModel : class;

        /// <summary>
        /// Gets the value of the cache if it is set. Otherwise initializes the cache with the Func.
        /// </summary>
        /// <typeparam name="TModel">THe type of the data</typeparam>
        /// <param name="key">The key under which to save the cache</param>
        /// <param name="func">The func to initialize the data, in the event that the data is not in the cache</param>
        /// <returns>Returns the cache data</returns>
        Task<TModel> GetOrSet<TModel>(string key, Func<Task<TModel>> func)
            where TModel : class;

        /// <summary>
        /// Delete a specific key
        /// </summary>
        /// <param name="key">The key to delete from the cache</param>
        void Delete(string key);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Clear();
    }
}
