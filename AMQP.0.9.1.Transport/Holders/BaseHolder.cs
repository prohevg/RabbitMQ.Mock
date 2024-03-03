using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AMQP_0_9_1.Transport.Holders
{
    public abstract class BaseHolder<T> : IBaseHolder<T>
    {
        private readonly ConcurrentDictionary<string, T> _list = new();

        #region IBaseHolder

        public void Add(string key, T item)
        {
            _list.TryAdd(key, item);
        }

        /// <summary>
        /// Get value by key if exists
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>true if exists value</returns>
        public bool TryGetValue(string key, out T value)
        {
            return _list.TryGetValue(key, out value);
        }

        public void Remove(string key)
        {
            _list.TryRemove(key, out _);
        }

        public IEnumerable<T> List => _list.Values;

        #endregion
    }
}
