using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public interface IMinimalDictionary<TKey, TValue>
    {
        void Add(TKey key, TValue value);
        bool ContainsKey(TKey key);
        bool Remove(TKey key);
        bool TryGetValue(TKey key, out TValue value);
        void Clear();

        IEnumerable<TKey> GetKeys();
        IEnumerable<TValue> GetValues();
    }

    public interface IMinimalDictionary
    {
        void Add(object key, object value);
        bool ContainsKey(object key);
        bool Remove(object key);
        bool TryGetValue(object key, out object value);
        void Clear();

        IEnumerable<object> GetKeys();
        IEnumerable<object> GetValues();
    }

    public static class MinimalDictionaryExtensions
    {
        public static TValue GetValue<TKey, TValue>(this IMinimalDictionary<TKey, TValue> minimalDictionary, TKey key)
        {
            TValue value;

            if (minimalDictionary.TryGetValue(key, out value))
            {
                return value;
            }

            throw new Granular.Exception("The given key was not present in the dictionary.");
        }

        public static object GetValue(this IMinimalDictionary minimalDictionary, object key)
        {
            object value;

            if (minimalDictionary.TryGetValue(key, out value))
            {
                return value;
            }

            throw new Granular.Exception("The given key was not present in the dictionary.");
        }
    }

    public class MinimalDictionary<TKey, TValue> : IMinimalDictionary<TKey, TValue>, IMinimalDictionary
    {
        private static readonly TValue DefaultValue = default(TValue);

        private Dictionary<object, object> dictionary;

        public MinimalDictionary(IEqualityComparer<TKey> comparer = null)
        {
            this.dictionary = new Dictionary<object, object>((IEqualityComparer<object>)comparer);
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            object result;
            if (dictionary.TryGetValue(key, out result))
            {
                value = (dynamic)result;
                return true;
            }

            value = DefaultValue;
            return false;
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        void IMinimalDictionary.Add(object key, object value)
        {
            dictionary.Add(key, value);
        }

        bool IMinimalDictionary.ContainsKey(object key)
        {
            return dictionary.ContainsKey(key);
        }

        bool IMinimalDictionary.Remove(object key)
        {
            return dictionary.Remove(key);
        }

        bool IMinimalDictionary.TryGetValue(object key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public IEnumerable<TKey> GetKeys()
        {
            return ((IDictionary<object, object>)dictionary).Keys.Cast<TKey>();
        }

        public IEnumerable<TValue> GetValues()
        {
            return ((IDictionary<object, object>)dictionary).Values.Cast<TValue>();
        }

        IEnumerable<object> IMinimalDictionary.GetKeys()
        {
            return ((IDictionary<object, object>)dictionary).Keys;
        }

        IEnumerable<object> IMinimalDictionary.GetValues()
        {
            return ((IDictionary<object, object>)dictionary).Values;
        }
    }
}
