using System;
using System.Collections.Generic;

namespace BL.CrossCutting.Helpers.CashService
{
    public class CasheBase<K, T> : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBase{K,T}"/> class.
        /// </summary>
        public CasheBase()
        {
        }

        private readonly Dictionary<K, T> _cache = new Dictionary<K, T>();

        #region IDisposable implementation & Clear

        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    // Dispose managed resources.
                    Clear();
                }
                // Dispose unmanaged resources
            }
        }

        /// <summary>
        /// Clears the entire _cache and disposes all active timers.
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        #endregion


        #region AddOrUpdate, Get, Remove, Exists, Clear

        /// <summary>
        /// Adds or updates the specified _cache-key with the specified cacheObject and applies <c>Timeout.Infinite</c> to this key.
        /// </summary>
        /// <param name="key">The _cache-key to add or update.</param>
        /// <param name="cacheObject">The _cache object to store.</param>
        public void AddOrUpdate(K key, T cacheObject)
        {
            if (_disposed) return;

            if (!_cache.ContainsKey(key))
                _cache.Add(key, cacheObject);
            else
                _cache[key] = cacheObject;

        }

        /// <summary>
        /// Gets the _cache entry with the specified key or returns <c>default(T)</c> if the key is not found.
        /// </summary>
        /// <param name="key">The _cache-key to retrieve.</param>
        /// <returns>The object from the _cache or <c>default(T)</c>, if not found.</returns>
        public T this[K key] => Get(key);

        /// <summary>
        /// Gets the _cache entry with the specified key or return <c>default(T)</c> if the key is not found.
        /// </summary>
        /// <param name="key">The _cache-key to retrieve.</param>
        /// <returns>The object from the _cache or <c>default(T)</c>, if not found.</returns>
        public T Get(K key)
        {
            if (_disposed) return default(T);

            T rv;
            return (_cache.TryGetValue(key, out rv) ? rv : default(T));

        }

        /// <summary>
        /// Tries to gets the _cache entry with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">(out) The value, if found, or <c>default(T)</c>, if not.</param>
        /// <returns><c>True</c>, if <c>key</c> exists, otherwise <c>false</c>.</returns>
        public bool TryGet(K key, out T value)
        {
            if (_disposed)
            {
                value = default(T);
                return false;
            }
            return _cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes the specified _cache entry with the specified key.
        /// If the key is not found, no exception is thrown, the statement is just ignored.
        /// </summary>
        /// <param name="key">The _cache-key to remove.</param>
        public void Remove(K key)
        {
            if (_disposed) return;
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Checks if a specified key exists in the _cache.
        /// </summary>
        /// <param name="key">The _cache-key to check.</param>
        /// <returns><c>True</c> if the key exists in the _cache, otherwise <c>False</c>.</returns>
        public bool Exists(K key)
        {
            if (_disposed) return false;
            return _cache.ContainsKey(key);
        }

        #endregion
    }

}