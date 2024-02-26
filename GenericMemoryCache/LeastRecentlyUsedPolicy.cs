using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache
{
    /// <summary>
    /// Implements the Least Recently Used policy to evicts items from the cache. The oldest item that has not been used will be the one to evict. If a cache item is accessed, 
    /// it becomes the Most Recently used and therefore, the last one to be evicted.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LeastRecentlyUsedPolicy<T> : ICachePolicy<T> where T : notnull
    {
        private LinkedList<T> _recentlyUsed;
        
        private int _maximumCapacity;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maximumCapacity">Maximum capacity of the cache.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the maximum capacity is 0 or less.</exception>
        public LeastRecentlyUsedPolicy(int maximumCapacity)
        {
            if (maximumCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCapacity), "Maximum capacity must be greater than 0.");
            }

            this._maximumCapacity = maximumCapacity;
            this._recentlyUsed = new LinkedList<T>();
        }

        /// <summary>
        /// It will add the item index in the cache if it is not in the cache, becoming the most recently used item of the cache. 
        /// If the item exists, it will become the newest cache element of the cahe.
        /// If a new element is added and the cache is full, the least element used will be removed.
        /// </summary>
        /// <param name="index">Cache item index.</param>
        /// <param name="indexRemoved">Cache item index removed from the cache.</param>
        /// <returns></returns>
        public bool AddOrUpdateIndex(T index, out T indexRemoved)
        {            
            bool removed = false;
            indexRemoved = default(T)!;

            //indexRemoved = (T)this._recentlyUsed.DefaultIfEmpty<T>();

            var found = this._recentlyUsed.Find(index);

            if (found != null)
            {
                this._recentlyUsed.Remove(found);
                this._recentlyUsed.AddLast(index);
            }
            else
            {
                if (this._recentlyUsed.Count == this._maximumCapacity)
                {
                    indexRemoved = this._recentlyUsed.First();
                    this._recentlyUsed.Remove(indexRemoved);

                    removed = true;
                }

                this._recentlyUsed.AddLast(index);
            }

            return removed;
        }

        /// <summary>
        /// Clears the cache indexes.
        /// </summary>
        public void Clear()
        {
            this._recentlyUsed.Clear();
        }

        ~LeastRecentlyUsedPolicy()
        {
            this.Clear();
        }
    }
}
