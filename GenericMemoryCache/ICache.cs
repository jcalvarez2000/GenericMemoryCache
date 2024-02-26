namespace Cache
{ 
   
    /// <summary>
    /// Interface for Memory Caches
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICache<T> where T : notnull
    {
        /// <summary>
        /// Configuration of the cache
        /// </summary>
        ICacheConfiguration Configuration { get; }

        /// <summary>
        /// Event that is triggered when an item of the cache is evicted.
        /// </summary>
        event CacheItemRemoved? ItemRemoved;

        /// <summary>
        /// Handler for the event <see cref="ItemRemoved"/>
        /// </summary>
        /// <param name="key"></param>
        delegate void CacheItemRemoved(T key);

        /// <summary>
        /// Reads the cache item from the cache which index is <paramref name="cacheItem"/>.
        /// </summary>
        /// <param name="key">Cache index to read.</param>
        /// <param name="cacheItem">Cache item read.</param>
        /// <returns>True if the element exists in the cache. False, otherwise.</returns>
        bool Read(T key, out object? cacheItem);

        /// <summary>
        /// Writes a cache item in the cache.
        /// </summary>
        /// <param name="key">Index of the cache item in the memory cache.</param>
        /// <param name="cacheItem">Item written in the cache.</param>
        void Write(T key, object cacheItem);

        /// <summary>
        /// Clears the memory cache. 
        /// </summary>
        void Clear();
    }
}