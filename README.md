# GenericMemoryCache
Generic Memory Cache with a limit threadshold of items that follows a Least Recently Used policy to evict cache items when the memory is full.

The cache can be indexed by a generic type T, and any element can be stored associated with that indes.

## Configuration
The cache can be configured with:

<ul>
  <li><strong>MaxItems:</strong> Maximum cache items allowed in the cache. </li>
  <li><strong>CachePolicy:</strong> Policiy used to evict elements from the cache. At this moment, only Least Recently Used policy is available to use</li>
</ul>

## Least Recently Used policy
With this policy, the element of the cache that has been least read or write will be evicted from the cache.

When a new element is written in the cache, or an existing item is updated or read in the cache, this element becomes the most used element.

If a new element is added and the cache is full, the least element used will be removed.
