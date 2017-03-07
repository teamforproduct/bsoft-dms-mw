﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BL.CrossCutting.Helpers.CashService
{
    public class CacheBase<K, T> : IDisposable
    {
        #region Constructor and class members
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBaseBase{K,T}"/> class.
        /// </summary>
        public CacheBase() { }

        private Dictionary<K, T> cache = new Dictionary<K, T>();
        //private Dictionary<K, Timer> timers = new Dictionary<K, Timer>();
        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        #endregion

        #region IDisposable implementation & Clear
        private bool disposed = false;

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
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    // Dispose managed resources.
                    Clear();
                    locker.Dispose();
                }
                // Dispose unmanaged resources
            }
        }

        /// <summary>
        /// Clears the entire cache and disposes all active timers.
        /// </summary>
        public void Clear()
        {
            locker.EnterWriteLock();
            try
            {
                //try
                //{
                //    foreach (Timer t in timers.Values)
                //        t.Dispose();
                //}
                //catch
                //{ }

                //timers.Clear();
                cache.Clear();
            }
            finally { locker.ExitWriteLock(); }
        }
        #endregion

        //#region CheckTimer
        //// Checks whether a specific timer already exists and adds a new one, if not 
        //private void CheckTimer(K key, int cacheTimeout, bool restartTimerIfExists)
        //{
        //    Timer timer;

        //    if (timers.TryGetValue(key, out timer))
        //    {
        //        if (restartTimerIfExists)
        //        {
        //            timer.Change(
        //                (cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000),
        //                Timeout.Infinite);
        //        }
        //    }
        //    else
        //        timers.Add(
        //            key,
        //            new Timer(
        //                new TimerCallback(RemoveByTimer),
        //                key,
        //                (cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000),
        //                Timeout.Infinite));
        //}

        //private void RemoveByTimer(object state)
        //{
        //    Remove((K)state);
        //}
        //#endregion

        #region AddOrUpdate, Get, Remove, Exists, Clear
        ///// <summary>
        ///// Adds or updates the specified cache-key with the specified cacheObject and applies a specified timeout (in seconds) to this key.
        ///// </summary>
        ///// <param name="key">The cache-key to add or update.</param>
        ///// <param name="cacheObject">The cache object to store.</param>
        ///// <param name="cacheTimeout">The cache timeout (lifespan) of this object. Must be 1 or greater.
        ///// Specify Timeout.Infinite to keep the entry forever.</param>
        ///// <param name="restartTimerIfExists">(Optional). If set to <c>true</c>, the timer for this cacheObject will be reset if the object already
        ///// exists in the cache. (Default = false).</param>
        //public void AddOrUpdate(K key, T cacheObject, int cacheTimeout, bool restartTimerIfExists = false)
        //{
        //    if (disposed) return;

        //    if (cacheTimeout != Timeout.Infinite && cacheTimeout < 1)
        //        throw new ArgumentOutOfRangeException("cacheTimeout must be greater than zero.");

        //    locker.EnterWriteLock();
        //    try
        //    {
        //        //CheckTimer(key, cacheTimeout, restartTimerIfExists);

        //        if (!cache.ContainsKey(key))
        //            cache.Add(key, cacheObject);
        //        else
        //            cache[key] = cacheObject;
        //    }
        //    finally { locker.ExitWriteLock(); }
        //}

        /// <summary>
        /// Adds or updates the specified cache-key with the specified cacheObject and applies <c>Timeout.Infinite</c> to this key.
        /// </summary>
        /// <param name="key">The cache-key to add or update.</param>
        /// <param name="cacheObject">The cache object to store.</param>
        public void AddOrUpdate(K key, T cacheObject)
        {
            //AddOrUpdate(key, cacheObject, Timeout.Infinite);
            if (disposed) return;

            locker.EnterWriteLock();
            try
            {
                if (!cache.ContainsKey(key))
                    cache.Add(key, cacheObject);
                else
                    cache[key] = cacheObject;
            }
            finally { locker.ExitWriteLock(); }
        }

        /// <summary>
        /// Gets the cache entry with the specified key or returns <c>default(T)</c> if the key is not found.
        /// </summary>
        /// <param name="key">The cache-key to retrieve.</param>
        /// <returns>The object from the cache or <c>default(T)</c>, if not found.</returns>
        public T this[K key] => Get(key);

        /// <summary>
        /// Gets the cache entry with the specified key or return <c>default(T)</c> if the key is not found.
        /// </summary>
        /// <param name="key">The cache-key to retrieve.</param>
        /// <returns>The object from the cache or <c>default(T)</c>, if not found.</returns>
        public T Get(K key)
        {
            if (disposed) return default(T);

            locker.EnterReadLock();
            try
            {
                T rv;
                return (cache.TryGetValue(key, out rv) ? rv : default(T));
            }
            finally { locker.ExitReadLock(); }
        }

        /// <summary>
        /// Tries to gets the cache entry with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">(out) The value, if found, or <c>default(T)</c>, if not.</param>
        /// <returns><c>True</c>, if <c>key</c> exists, otherwise <c>false</c>.</returns>
        public bool TryGet(K key, out T value)
        {
            if (disposed)
            {
                value = default(T);
                return false;
            }

            locker.EnterReadLock();
            try
            {
                return cache.TryGetValue(key, out value);
            }
            finally { locker.ExitReadLock(); }
        }

        /// <summary>
        /// Removes a series of cache entries in a single call for all key that match the specified key pattern.
        /// </summary>
        /// <param name="keyPattern">The key pattern to remove. The Predicate has to return true to get key removed.</param>
        public void Remove(Predicate<K> keyPattern)
        {
            if (disposed) return;

            locker.EnterWriteLock();
            try
            {
                var removers = (from k in cache.Keys.Cast<K>()
                                where keyPattern(k)
                                select k).ToList();

                foreach (K workKey in removers)
                {
                    //try { timers[workKey].Dispose(); }
                    //catch { }
                    //timers.Remove(workKey);
                    cache.Remove(workKey);
                }
            }
            finally { locker.ExitWriteLock(); }
        }

        /// <summary>
        /// Removes the specified cache entry with the specified key.
        /// If the key is not found, no exception is thrown, the statement is just ignored.
        /// </summary>
        /// <param name="key">The cache-key to remove.</param>
        public void Remove(K key)
        {
            if (disposed) return;

            locker.EnterWriteLock();
            try
            {
                if (cache.ContainsKey(key))
                {
                    //try { timers[key].Dispose(); }
                    //catch { }
                    //timers.Remove(key);
                    cache.Remove(key);
                }
            }
            finally { locker.ExitWriteLock(); }
        }

        /// <summary>
        /// Checks if a specified key exists in the cache.
        /// </summary>
        /// <param name="key">The cache-key to check.</param>
        /// <returns><c>True</c> if the key exists in the cache, otherwise <c>False</c>.</returns>
        public bool Exists(K key)
        {
            if (disposed) return false;

            locker.EnterReadLock();
            try
            {
                return cache.ContainsKey(key);
            }
            finally { locker.ExitReadLock(); }
        }
        #endregion
    }

}