using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public class SortedList<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private static readonly TValue DefaultValue = default(TValue);

        private List<TKey> keys;
        private ICollection<TKey> readOnlyKeys;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys { get { return readOnlyKeys; } }

        private List<TValue> values;
        private ICollection<TValue> readOnlyValues;
        ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return readOnlyValues; } }

        public TValue this[TKey key]
        {
            get
            {
                int index;
                if (!FindItem(key, out index))
                {
                    throw new Granular.Exception("Item with key \"{0}\" does not exist", key);
                }

                return values[index];
            }
            set
            {
                int index;
                if (!FindItem(key, out index))
                {
                    keys.Insert(index, key);
                    values.Insert(index, value);
                }
                else
                {
                    keys[index] = key;
                    values[index] = value;
                }
            }
        }

        public int Count { get { return keys.Count; } }

        public bool IsReadOnly { get { return false; } }

        private IComparer<TKey> comparer;

        public SortedList(IComparer<TKey> comparer)
        {
            this.comparer = comparer;

            keys = new List<TKey>();
            readOnlyKeys = new ReadOnlyCollection<TKey>(keys);

            values = new List<TValue>();
            readOnlyValues = new ReadOnlyCollection<TValue>(values);
        }

        public bool ContainsKey(TKey key)
        {
            int index;
            return FindItem(key, out index);
        }

        public void Add(TKey key, TValue value)
        {
            int index;
            if (FindItem(key, out index))
            {
                throw new Granular.Exception("Item with key \"{0}\" already exists", key);
            }

            keys.Insert(index, key);
            values.Insert(index, value);
        }

        public bool Remove(TKey key)
        {
            int index;
            if (!FindItem(key, out index))
            {
                return false;
            }

            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }

        public bool RemoveAt(int index)
        {
            if (index >= Count)
            {
                return false;
            }

            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index;
            if (!FindItem(key, out index))
            {
                value = DefaultValue;
                return false;
            }

            value = values[index];
            return true;
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        private bool FindItem(TKey key, out int closestIndex)
        {
            return FindItem(key, 0, keys.Count - 1, out closestIndex);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        private bool FindItem(TKey key, int firstIndex, int lastIndex, out int closestIndex)
        {
            if (firstIndex > lastIndex)
            {
                closestIndex = firstIndex;
                return false;
            }

            int middleIndex = (firstIndex + lastIndex) / 2;
            int compareResult = comparer.Compare(key, keys[middleIndex]);

            if (compareResult < 0)
            {
                return FindItem(key, firstIndex, middleIndex - 1, out closestIndex);
            }

            if (compareResult > 0)
            {
                return FindItem(key, middleIndex + 1, lastIndex, out closestIndex);
            }

            closestIndex = middleIndex;
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return keys.Zip(values, (key, value) => new KeyValuePair<TKey, TValue>(key, value)).GetEnumerator();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
