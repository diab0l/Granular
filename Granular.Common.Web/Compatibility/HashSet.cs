using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public class HashSet<T> : ICollection<T>
    {
        public int Count { get { return dictionary.Count; } }

        public bool IsReadOnly { get { return false; } }

        private Dictionary<T, object> dictionary;

        public HashSet()
        {
            dictionary = new Dictionary<T, object>();
        }

        public bool Contains(T item)
        {
            return dictionary.ContainsKey(item);
        }

        public void Add(T item)
        {
            dictionary[item] = null;
        }

        public bool Remove(T item)
        {
            return dictionary.Remove(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            //
        }
    }
}
