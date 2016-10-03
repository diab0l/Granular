using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public class PriorityQueue<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private static readonly TValue DefaultValue = default(TValue);

        public int Count { get { return items.Count; } }

        private IComparer<TKey> comparer;
        private List<KeyValuePair<TKey, TValue>> items;

        public PriorityQueue() :
            this(Compatibility.Comparer<TKey>.Default)
        {
            //
        }

        public PriorityQueue(IComparer<TKey> comparer)
        {
            this.comparer = comparer;
            this.items = new List<KeyValuePair<TKey, TValue>>();
        }

        public void Enqueue(TKey key, TValue value)
        {
            items.Insert(GetKeyIndex(key, 0, items.Count), new KeyValuePair<TKey, TValue>(key, value));
        }

        private int GetKeyIndex(TKey key, int startIndex, int endIndex)
        {
            if (endIndex - startIndex == 0)
            {
                return endIndex;
            }

            if (endIndex - startIndex == 1)
            {
                return comparer.Compare(key, items[startIndex].Key) > 0 ? startIndex : endIndex;
            }

            int middleIndex = (startIndex + endIndex) / 2;

            return comparer.Compare(key, items[middleIndex].Key) > 0 ?
                GetKeyIndex(key, startIndex, middleIndex) :
                GetKeyIndex(key, middleIndex, endIndex);
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

        public bool TryDequeue(out TValue value)
        {
            if (TryPeek(out value))
            {
                items.RemoveAt(0);
                return true;
            }

            value = DefaultValue;
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

        public bool TryPeek(out TValue value)
        {
            if (items.Count > 0)
            {
                value = items[0].Value;
                return true;
            }

            value = DefaultValue;
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
