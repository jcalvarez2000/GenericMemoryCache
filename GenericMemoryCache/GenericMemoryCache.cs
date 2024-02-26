using Microsoft.VisualBasic.FileIO;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Cache
{
    /// <summary>
    /// Generic Memory Cache which items can be accessed with key of type T.
    /// The cache will have a maximum capacity of items. 
    /// 
    /// Writing in the cache will be always possible. If the cache is full, the element to remove from the cache will be selected according 
    /// to the <see cref="ICachePolicy{T}"/> used. At this moment, only the <see cref="LeastRecentlyUsedPolicy"/> is implemented.
    /// 
    /// The maximum capacity of items and the cache policy are configurable when instantiating the class.
    /// 
    /// A Singleton Factory is used to instantiate the class and make sure there is only one instance of the cache. 
    /// </summary>
    /// <typeparam name="T">Type of the index to use to access the cache.</typeparam>
    public class GenericMemoryCache<T> : ICache<T> where T : notnull
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GenericMemoryCache<T>? Instance { get; private set; }

        /// <inheritdoc cref="ICache{T}.Configuration"/>
        public ICacheConfiguration Configuration { get; private set; }

        /// <inheritdoc cref="ICache{T}.ItemRemoved"/>
        public event ICache<T>.CacheItemRemoved? ItemRemoved;

        /// <inheritdoc cref="ICache{T}.Write(T, object)"/>
        public void Write(T key, object cacheItem)
        {
            semaphore.Wait();

            T indexRemoved;
            if (this._cachePolicy!.AddOrUpdateIndex(key, out indexRemoved))
            {
                _ = this._cache!.Remove(indexRemoved, value: out _);
                this.ItemRemoved?.Invoke(indexRemoved);
            }

            this._cache![key] = cacheItem;

            semaphore.Release();
        }

        /// <inheritdoc cref="ICache{T}.Read(T, out object?)"/>
        public bool Read(T key, out object? cacheItem)
        {
            semaphore.Wait();

            var itemExistsInCache = this._cache!.TryGetValue(key, out cacheItem);

            if (itemExistsInCache)
            {
                if (this._cachePolicy!.AddOrUpdateIndex(key, out var indexRemoved))
                {
                    throw new ArgumentOutOfRangeException($"Cache item with index {key} was not found in the Cache Policy. Element with index {indexRemoved} was removed from the Cache Policy");
                }
            }
            semaphore.Release();

            return itemExistsInCache;
        }

        /// <inheritdoc cref="ICache{T}.Clear"/>
        public void Clear()
        {
            this._cache?.Clear();
            this._cachePolicy?.Clear();
        }

        private ConcurrentDictionary<T, object>? _cache;
        private ICachePolicy<T>? _cachePolicy;

        private static readonly SemaphoreSlim semaphore = new(1, 1);

        private GenericMemoryCache(ICacheConfiguration configuration, ICachePolicy<T> cachePolicy)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(cachePolicy);

            this.Configuration = configuration;

            this._cachePolicy = cachePolicy;
            this._cache = new ConcurrentDictionary<T, object>(1, configuration.MaxItems);
        }

        ~GenericMemoryCache()
        {
            this.Clear();
        }

        /// <summary>
        /// Factory to create a singleton instance for <see cref="GenericMemoryCache{T}"/>
        /// </summary>
        public class Factory
        {
            /// <summary>
            /// Creates a singleton instance for <see cref="GenericMemoryCache{T}"/> based on the configuration 
            /// </summary>
            /// <param name="configuration">Configuration used to set up the <see cref="GenericMemoryCache{T}"/> behaviour.</param>
            /// <returns>An instance of <see cref="GenericMemoryCache{T}"/></returns>
            public static GenericMemoryCache<T> Create(ICacheConfiguration configuration)
            {
                if (GenericMemoryCache<T>.Instance != null)
                {
                    return GenericMemoryCache<T>.Instance;
                }

                ICachePolicy<T> cachePolicy = configuration.CachePolicy switch
                {
                    CachePolicies.LeastRecentlyUsed => new LeastRecentlyUsedPolicy<T>(configuration.MaxItems),
                    _ => new LeastRecentlyUsedPolicy<T>(configuration.MaxItems),
                };

                GenericMemoryCache<T>.Instance = new GenericMemoryCache<T>(configuration, cachePolicy);

                return GenericMemoryCache<T>.Instance;
            }

            /// <summary>
            /// Destroys the singleton instance of  <see cref="GenericMemoryCache{T}"/> 
            /// </summary>
            public static void Destroy()
            {
                GenericMemoryCache<T>.Instance?.Clear();

                GenericMemoryCache<T>.Instance = null;
            }            
        }
    }
}