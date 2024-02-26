using Cache;

void ItemRemovedHandler(int key)
{
    Console.WriteLine($"Cache item with index {key} has been removed.");
}

var configuration = new GenericMemoryCacheConfiguration { MaxItems = 3 };

var cache = GenericMemoryCache<int>.Factory.Create(configuration);

cache.ItemRemoved += ItemRemovedHandler;

Console.WriteLine($"Maximum size of the cache is: {cache.Configuration.MaxItems}");

// Fill the cache with data
for (var i = 0; i < cache.Configuration.MaxItems; ++i)
{
    var cacheItem = $"Object with index {i}";

    cache.Write(i, cacheItem);

    Console.WriteLine($"Element added: {cacheItem}");
}

// Elements in the cache [0, 1, 2]
// Order of use from oldest to new [0, 1, 2]

// Read elements from the cache
for (var i = 0; i < cache.Configuration.MaxItems; ++i)
{
    object? cacheItem;
    if (cache.Read(i, out cacheItem))
    {
        Console.WriteLine($"Element with index {i} was found in the cache with content: [{cacheItem}]");
    }
    else
    {
        Console.WriteLine($"ERROR: Element with index {i} was NOT found.");
    }
}

// Elements in the cache [0, 1, 2]
// Order of use from oldest to new[0, 1, 2]

// Update the "first" element that was accessed in the cache
var cacheItemUpdated = "Object with index 0 updated";
cache.Write(0, cacheItemUpdated);

// Elements in the cache [0, 1, 2]
// Order of use from oldest to new [1, 2, 0]

object? retrievedCacheItem;
if (cache.Read(0, out retrievedCacheItem))
{
    Console.WriteLine($"Element with index {0} was found in the cache with content: [{retrievedCacheItem}]");
}
else
{
    Console.WriteLine($"ERROR: Element with index {0} was NOT found.");
}

// Elements in the cache [0, 1, 2]
// Order of use from oldest to new [1, 2, 0]

// Add a new element, the item with index 1 should be removed as it is the least used
var newElementIndex = cache.Configuration.MaxItems;
cacheItemUpdated = $"New object with index {newElementIndex}";

cache.Write(cache.Configuration.MaxItems, cacheItemUpdated);

// Elements in the cache [0, 2, 3]
// Order of use from oldest to new[2, 0, 3]

if (cache.Read(2, out retrievedCacheItem))
{
    Console.WriteLine($"Element with index {0} was found in the cache with content: [{retrievedCacheItem}]");
}
else
{
    Console.WriteLine($"ERROR: Element with index {0} was NOT found.");
}

// Elements in the cache [0, 2, 3]
// Order of use from less to more [0, 3, 2]

newElementIndex = cache.Configuration.MaxItems + 1;
cacheItemUpdated = $"New object with index {newElementIndex}";

cache.Write(newElementIndex, cacheItemUpdated);

// Elements in the cache [2, 3, 4]
// Order of use from less to more [3, 2, 4]

GenericMemoryCache<int>.Factory.Destroy();

Console.WriteLine("Pulse una tecla para finalizar");
Console.ReadKey(false);

