using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;

namespace System.Windows
{
    public class FreezableCollection<T> : Freezable, IObservableCollection<T>, IList<T> where T : DependencyObject
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count { get { return collection.Count; } }

        public T this[int index]
        {
            get { return collection[index]; }
            set { collection[index] = value; }
        }

        public bool IsReadOnly { get { return collection.IsReadOnly; } }

        private ObservableCollection<T> collection;

        public FreezableCollection() :
            this(new T[0])
        {
            //
        }

        public FreezableCollection(IEnumerable<T> collection)
        {
            this.collection = new ObservableCollection<T>(collection);
            this.collection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);

            foreach (T value in collection)
            {
                if (value is IContextElement)
                {
                    ((IContextElement)value).TrySetContextParent(this);
                }

                if (value is INotifyChanged)
                {
                    ((INotifyChanged)value).Changed += OnItemChanged;
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (T value in e.OldItems)
            {
                if (value is IContextElement)
                {
                    ((IContextElement)value).TrySetContextParent(null);
                }

                if (value is INotifyChanged)
                {
                    ((INotifyChanged)value).Changed -= OnItemChanged;
                }
            }

            foreach (T value in e.NewItems)
            {
                if (value is IContextElement)
                {
                    ((IContextElement)value).TrySetContextParent(this);
                }

                if (value is INotifyChanged)
                {
                    ((INotifyChanged)value).Changed += OnItemChanged;
                }
            }

            CollectionChanged.Raise(this, e);
            RaiseChanged();
        }

        private void OnItemChanged(object sender, EventArgs e)
        {
            RaiseChanged();
        }

        public void Add(T item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        public int IndexOf(T item)
        {
            return collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            collection.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return collection.Remove(item);
        }

        public void RemoveAt(int index)
        {
            collection.RemoveAt(index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)collection).GetEnumerator();
        }
    }
}
