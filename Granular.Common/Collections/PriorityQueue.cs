using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public class PriorityQueue<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private class IndexedKey
        {
            public TKey Key { get; private set; }
            public int Index { get; private set; }

            public IndexedKey(TKey key, int index)
            {
                this.Key = key;
                this.Index = index;
            }
        }

        private class IndexedKeyComparer : IComparer<IndexedKey>
        {
            private IComparer<TKey> comparer;

            public IndexedKeyComparer(IComparer<TKey> comparer)
            {
                this.comparer = comparer;
            }

            public int Compare(IndexedKey x, IndexedKey y)
            {
                int result = comparer.Compare(x.Key, y.Key);
                return result != 0 ? result : Comparer<int>.Default.Compare(x.Index, y.Index);
            }
        }

        public int Count { get { return list.Count; } }

        private SortedList<IndexedKey, TValue> list;
        private int currentIndex;

        public PriorityQueue() :
            this(Compatibility.Comparer<TKey>.Default)
        {
            //
        }

        public PriorityQueue(IComparer<TKey> comparer)
        {
            this.list = new SortedList<IndexedKey, TValue>(new IndexedKeyComparer(comparer));
        }

        public void Enqueue(TKey key, TValue value)
        {
            list.Add(new IndexedKey(key, currentIndex++), value);
        }

        public TValue Dequeue()
        {
            TValue value;
            if (TryDequeue(out value))
            {
                return value;
            }

            throw new InvalidOperationException("Queue is empty");
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryDequeue(out TValue value)
        {
            if (TryPeek(out value))
            {
                list.RemoveAt(0);
                return true;
            }

            value = default(TValue);
            return false;
        }

        public TValue Peek()
        {
            TValue value;
            if (TryPeek(out value))
            {
                return value;
            }

            throw new InvalidOperationException("Queue is empty");
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryPeek(out TValue value)
        {
            if (list.Count > 0)
            {
                value = list.GetValues().First();
                return true;
            }

            value = default(TValue);
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return list.Select(pair => new KeyValuePair<TKey, TValue>(pair.Key.Key, pair.Value)).GetEnumerator();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
