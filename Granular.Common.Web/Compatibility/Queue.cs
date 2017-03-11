using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class Queue<T> : IEnumerable<T>
    {
        public int Count { get { return list.Count; } }

        private List<T> list;

        public Queue()
        {
            list = new List<T>();
        }

        public void Enqueue(T item)
        {
            list.Add(item);
        }

        public T Dequeue()
        {
            T item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public T Peek()
        {
            return list[0];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
