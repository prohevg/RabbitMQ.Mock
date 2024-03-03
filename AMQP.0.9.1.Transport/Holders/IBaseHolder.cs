using System.Collections.Generic;

namespace AMQP_0_9_1.Transport.Holders
{
    public interface IBaseHolder<T>
    {
        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="key">Item key</param>
        /// <param name="item">Item value</param>
        void Add(string key, T item);

        /// <summary>
        /// Get value by key if exists
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>true if exists value</returns>
        bool TryGetValue(string key, out T value);

        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="key">Item key</param>
        void Remove(string key);

        /// <summary>
        /// Items list
        /// </summary>
        IEnumerable<T> List { get; }
    }
}