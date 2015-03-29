using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public interface IListDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }
        void Add(TKey key, TValue value);
        bool Remove(TKey key, TValue value);
        IEnumerable<TValue> GetValues(TKey key);
    }

    public static class IListDictionaryExtensions
    {
        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this IListDictionary<TKey, TValue> listDictionary)
        {
            return listDictionary.Keys;
        }

        public static IEnumerable<TValue> GetValues<TKey, TValue>(this IListDictionary<TKey, TValue> listDictionary)
        {
            return listDictionary.Values;
        }

        public static bool Contains<TKey, TValue>(IListDictionary<TKey, TValue> listDictionary, TKey key, TValue value)
        {
            return listDictionary.GetValues(key).Contains(value);
        }
    }

    public class ListDictionary<TKey, TValue> : IListDictionary<TKey, TValue>
    {
        IEnumerable<TKey> IListDictionary<TKey, TValue>.Keys
        {
            get { return dictionary.Keys; }
        }

        IEnumerable<TValue> IListDictionary<TKey, TValue>.Values
        {
            get { return dictionary.SelectMany(pair => pair.Value); }
        }

        private Dictionary<TKey, List<TValue>> dictionary;

        public ListDictionary()
        {
            dictionary = new Dictionary<TKey, List<TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            List<TValue> list;

            if (!dictionary.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                dictionary.Add(key, list);
            }

            list.Add(value);
        }

        public bool Remove(TKey key, TValue value)
        {
            List<TValue> list;

            if (!dictionary.TryGetValue(key, out list))
            {
                return false;
            }

            if (!list.Remove(value))
            {
                return false;
            }

            if (list.Count == 0)
            {
                dictionary.Remove(key);
            }

            return true;
        }

        public IEnumerable<TValue> GetValues(TKey key)
        {
            return dictionary.ContainsKey(key) ? (IEnumerable<TValue>)dictionary[key] : new TValue[0];
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.SelectMany(pair => pair.Value.Select(value => new KeyValuePair<TKey, TValue>(pair.Key, value))).GetEnumerator();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
