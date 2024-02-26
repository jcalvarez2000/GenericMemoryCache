using Cache;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;

namespace Cache.Tests
{
    public class GenericMemoryCacheTests
    {
        private int? keyRemoved = null;
        private void ItemRemovedHandler(int key)
        {
            this.keyRemoved = key;
        }        

        [Theory]
        [InlineData("Key index 1", "Item 1", 1)]
        public void Write_InTheCacheWithAStringAsKey_ShouldWork
        (
            string key,
            object item,
            int maxItems
        )
        {
            // Arrange            
            var configuration = new GenericMemoryCacheConfiguration { MaxItems = maxItems };

            var sut = GenericMemoryCache<string>.Factory.Create(configuration);

            // Act
            sut.Write(key, item);

            // Assert
            object? writtenCacheItem;

            sut.Read(key, out writtenCacheItem).Should().BeTrue();

            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(item);

            GenericMemoryCache<int>.Factory.Destroy();
        }

        [Theory]
        [InlineData(0, "Item 1", 1)]
        public void Write_OneElementWithoutPassingMaximumSize_ShouldNotRemovedAnyElementFromCache
        (
            int key,
            object item,
            int maxItems
        )
        {
            // Arrange            
            var configuration = new GenericMemoryCacheConfiguration { MaxItems = maxItems };

            var sut = GenericMemoryCache<int>.Factory.Create(configuration);

            // Act
            sut.Write(key, item);

            // Assert
            object? writtenCacheItem;

            sut.Read(key, out writtenCacheItem).Should().BeTrue();

            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(item);

            GenericMemoryCache<int>.Factory.Destroy();
        }

        [Theory]
        [InlineData(new int[] {0, 1, 2 }, new string[] { "Item 1", "Item 2", "Item 3" }, 2)]
        public void Write_OneElementMoreThanTheCapacity_ShouldRemoveTheFirstOneWritten
        
            int[] keys,
            object[] items,
            int maxItems
        )
        {
            // Arrange
            keys.Length.Should().Be(items.Length);

            var configuration = new GenericMemoryCacheConfiguration { MaxItems = maxItems };

            var sut = GenericMemoryCache<int>.Factory.Create(configuration);

            // Act
            for (var i = 0; i < keys.Length; i++)
            {
                sut.Write(keys[i], items[i]);
            }
            
            // Assert
            object? writtenCacheItem;

            sut.Read(0, out writtenCacheItem).Should().BeFalse();
            
            sut.Read(1, out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[1]);

            sut.Read(2, out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[2]);

            GenericMemoryCache<int>.Factory.Destroy();
        }

        [Theory]
        [InlineData(new int[] { 0, 1 }, new string[] { "Item 1", "Item 2" }, 2)]
        public void Write_UpdatingAnExistingItemInCacheIdentifiedByAKey_ShouldUpdateItsContent
        (
            int[] keys,
            object[] items,
            int maxItems
        )
        {
            // Arrange
            keys.Length.Should().Be(items.Length);

            var configuration = new GenericMemoryCacheConfiguration { MaxItems = maxItems };

            var sut = GenericMemoryCache<int>.Factory.Create(configuration);

            // Act && Assert
            for (var i = 0; i < keys.Length; i++)
            {
                sut.Write(keys[i], items[i]);
            }

            object? writtenCacheItem;

            sut.Read(keys[0], out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[0]);

            sut.Read(keys[1], out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[1]);

            var newValue = "Item 1 updated";
            sut.Write(0, newValue);

            sut.Read(0, out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(newValue);

            GenericMemoryCache<int>.Factory.Destroy();
        }

        [Theory]
        [InlineData(new int[] { 0, 1, 2, 3, 1, 4 }, new string[] { "Item 1", "Item 2", "Item 3", "Item 4", "Updated Item 2", "Item 5" }, 3)]
        public void Write_UpdatingOldestExistingItemInCache_ShouldKeeptItInTheCacheForTheNextWrite
        (
            int[] keys,
            object[] items,
            int maxItems
        )
        {
            // Arrange
            keys.Length.Should().Be(items.Length);

            var configuration = new GenericMemoryCacheConfiguration { MaxItems = maxItems };           
            
            var sut = GenericMemoryCache<int>.Factory.Create(configuration);

            // Act && Assert
            for (var i = 0; i < keys.Length; i++)
            {
                sut.Write(keys[i], items[i]);
            }

            object? writtenCacheItem;

            sut.Read(keys[3], out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[3]);

            sut.Read(keys[4], out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[4]);

            sut.Read(keys[5], out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[5]);

            GenericMemoryCache<int>.Factory.Destroy();
        }

        [Theory]
        [InlineData(new int[] { 0, 1, 2}, new string[] { "Item 1", "Item 2", "Item 3" }, 3)]
        public void Write_AfterReadingTheEldestElement_ShouldAvoidEvictingEldestElement
        (
            int[] keys,
            object[] items,
            int maxItems
        )
        {
            // Arrange
            keys.Length.Should().Be(items.Length);

            var configuration = new GenericMemoryCacheConfiguration { MaxItems = maxItems };

            var sut = GenericMemoryCache<int>.Factory.Create(configuration);

            sut.ItemRemoved += this.ItemRemovedHandler;
            this.keyRemoved = null;

            // Act && Assert
            for (var i = 0; i < keys.Length; i++)
            {
                sut.Write(keys[i], items[i]);
            }

            object? writtenCacheItem;

            sut.Read(keys[0], out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[0]);

            var newItem = "New Item 4";
            sut.Write(3, newItem);

            this.keyRemoved.Should().NotBeNull();
            this.keyRemoved.Should().Be(1);

            sut.Read(keys[0], out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[0]);

            sut.Read(keys[1], out writtenCacheItem).Should().BeFalse();
            writtenCacheItem.Should().BeNull();

            sut.Read(3, out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(newItem);

            GenericMemoryCache<int>.Factory.Destroy();
        }

        [Theory]
        [InlineData(new int[] { 0, 1, }, new string[] { "Item 1", "Item 2" }, 2)]
        public void CreateGenericMemoryCache_CalledSeveralTimes_ShouldCreateOnlyOneInstance
        (
            int[] keys,
            object[] items,
            int maxItems
        )
        {
            // Arrange
            keys.Length.Should().Be(items.Length);

            var configuration = new GenericMemoryCacheConfiguration { MaxItems = maxItems };

            var sut = GenericMemoryCache<int>.Factory.Create(configuration);

            // Act && Assert
            sut.Write(keys[0], items[0]);
            
            object? writtenCacheItem;

            sut.Read(0, out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[0]);

            sut = GenericMemoryCache<int>.Factory.Create(configuration);

            sut.Write(keys[1], items[1]);

            sut.Read(0, out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[0]);

            sut.Read(1, out writtenCacheItem).Should().BeTrue();
            writtenCacheItem.Should().NotBeNull();
            writtenCacheItem.Should().Be(items[1]);

            GenericMemoryCache<int>.Factory.Destroy();
        }
    }
}