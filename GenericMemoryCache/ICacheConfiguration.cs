namespace Cache
{

    /// <summary>
    /// Interface to configure memory cache objects.
    /// </summary>
    public interface ICacheConfiguration
    {
        /// <summary>
        /// Cache policy to follow to evict cache items.
        /// </summary>
        CachePolicies CachePolicy { get; set; }

        /// <summary>
        /// Maximum items allowed in the memory cache.
        /// </summary>
        int MaxItems { get; set; }
    }
}