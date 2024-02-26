using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache
{
    /// <summary>
    /// Configuration for <see cref="GenericMemoryCache{T}"/> objects.
    /// </summary>
    public class GenericMemoryCacheConfiguration : ICacheConfiguration
    {
        /// <inheritdoc cref="ICacheConfiguration.MaxItems"/>
        public int MaxItems { get; set; }

        /// <inheritdoc cref="ICacheConfiguration.CachePolicy"/>
        public CachePolicies CachePolicy { get; set; } = CachePolicies.LeastRecentlyUsed;
    }
}
