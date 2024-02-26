namespace Cache
{
    /// <summary>
    /// Interface to keep the cache items order in the cache and evicts cache items from the cache following an especifi policy to select it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICachePolicy<T>
    {
        /// <summary>
        /// Adds an cache index if the cache item index does not exist or updates it if it exists.
        /// </summary>
        /// <param name="index">Cache item index.</param>
        /// <param name="indexRemoved">Cache item index removed from the cache when the cache is full.</param>
        /// <returns></returns>
        bool AddOrUpdateIndex(T index, out T indexRemoved);

        /// <summary>
        /// Clears the cache item indexes
        /// </summary>
        void Clear();
    }
}