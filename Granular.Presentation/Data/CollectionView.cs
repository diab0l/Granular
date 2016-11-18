using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows.Data
{
    public enum ListSortDirection
    {
        Ascending,
        Descending
    }

    public interface ICollectionView : IObservableCollection<object>
    {
        event EventHandler CurrentChanged;

        IEnumerable SourceCollection { get; }

        object CurrentItem { get; set; }
        int CurrentItemIndex { get; set; }

        bool CanFilter { get; }
        Func<object, bool> FilterPredicate { get; set; }

        bool CanSort { get; }
        Func<object, object> SortKeySelector { get; set; }
        ListSortDirection SortDirection { get; set; }
    }

    // ICollectionView adapter for an IEnumerable source
    public class CollectionView : ICollectionView, IDisposable
    {
        private class EmptyCollectionView : ICollectionView
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged { add { } remove { } }
            public event EventHandler CurrentChanged { add { } remove { } }
            public IEnumerable SourceCollection { get { return this; } }
            public object CurrentItem { get { return null; } set { } }
            public int CurrentItemIndex { get { return -1; } set { } }
            public bool CanFilter { get { return false; } }
            public Func<object, bool> FilterPredicate { get { return null; } set { } }
            public bool CanSort { get { return false; } }
            public Func<object, object> SortKeySelector { get { return null; } set { } }
            public ListSortDirection SortDirection { get { return ListSortDirection.Ascending; } set { } }

            public IEnumerator<object> GetEnumerator()
            {
                return Enumerable.Empty<object>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static readonly ICollectionView Empty = new EmptyCollectionView();

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler CurrentChanged;

        public IEnumerable SourceCollection { get; private set; }

        private object currentItem;
        public object CurrentItem
        {
            get { return currentItem; }
            set { SetCurrent(value); }
        }

        private int currentItemIndex;
        public int CurrentItemIndex
        {
            get { return currentItemIndex; }
            set { SetCurrent(value); }
        }

        public bool CanFilter { get { return true; } }

        private Func<object, bool> filterPredicate;
        public Func<object, bool> FilterPredicate
        {
            get { return filterPredicate; }
            set
            {
                if (filterPredicate == value)
                {
                    return;
                }

                filterPredicate = value;
                ResetInnerCollection();
            }
        }

        public bool CanSort { get { return true; } }

        private Func<object, object> sortKeySelector;
        public Func<object, object> SortKeySelector
        {
            get { return sortKeySelector; }
            set
            {
                if (sortKeySelector == value)
                {
                    return;
                }

                sortKeySelector = value;
                ResetInnerCollection();
            }
        }

        private ListSortDirection sortDirection;
        public ListSortDirection SortDirection
        {
            get { return sortDirection; }
            set
            {
                if (sortDirection == value)
                {
                    return;
                }

                sortDirection = value;
                ResetInnerCollection();
            }
        }

        private object[] innerCollection;

        public CollectionView(IEnumerable source)
        {
            this.SourceCollection = source;
            this.currentItemIndex = -1;

            if (SourceCollection is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)SourceCollection).CollectionChanged += OnSourceCollectionChanged;
            }

            ResetInnerCollection();
        }

        public void Dispose()
        {
            if (SourceCollection is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)SourceCollection).CollectionChanged -= OnSourceCollectionChanged;
            }
        }

        private void SetCurrent(object item)
        {
            int itemIndex = Granular.Compatibility.Array.IndexOf(innerCollection, item);

            if (itemIndex != -1)
            {
                SetCurrent(item, itemIndex);
            }
            else if (currentItemIndex >= 0 && currentItemIndex < innerCollection.Length)
            {
                SetCurrent(innerCollection[currentItemIndex], currentItemIndex);
            }
            else
            {
                SetCurrent(null, Math.Min(currentItemIndex, innerCollection.Length));
            }
        }

        private void SetCurrent(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < innerCollection.Length)
            {
                SetCurrent(innerCollection[itemIndex], itemIndex);
            }
            else
            {
                SetCurrent(null, Math.Min(itemIndex, innerCollection.Length));
            }
        }

        private void SetCurrent(object item, int itemIndex)
        {
            if (this.currentItem == item && this.currentItemIndex == itemIndex)
            {
                return;
            }

            this.currentItem = item;
            this.currentItemIndex = itemIndex;

            CurrentChanged.Raise(this);
        }

        private void ResetInnerCollection()
        {
            object[] oldInnerCollection = innerCollection ?? new object[0];

            innerCollection = TranslateCollection(Enumerable.Cast<object>(SourceCollection), filterPredicate, sortKeySelector, sortDirection).ToArray();

            if (innerCollection.Contains(CurrentItem))
            {
                SetCurrent(CurrentItem);
            }
            else
            {
                SetCurrent(CurrentItemIndex >= oldInnerCollection.Length ? innerCollection.Length : CurrentItemIndex);
            }

            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Reset(oldInnerCollection, innerCollection));
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            object[] oldInnerCollection = innerCollection;

            innerCollection = TranslateCollection(Enumerable.Cast<object>(SourceCollection), filterPredicate, sortKeySelector, sortDirection).ToArray();

            if (innerCollection.Contains(CurrentItem))
            {
                SetCurrent(CurrentItem);
            }
            else
            {
                SetCurrent(CurrentItemIndex >= oldInnerCollection.Length ? innerCollection.Length : CurrentItemIndex);
            }

            IEnumerable<object> oldItems = e.OldItems.Intersect(oldInnerCollection).ToArray();
            IEnumerable<object> newItems = e.NewItems.Intersect(innerCollection).ToArray();

            if (oldItems.Count() > 1 || newItems.Count() > 1)
            {
                CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Reset(oldInnerCollection, innerCollection));
                return;
            }

            object oldItem = e.OldItems.FirstOrDefault();
            int oldIndex = Granular.Compatibility.Array.FindIndex(oldInnerCollection, item => Granular.Compatibility.EqualityComparer.Default.Equals(item, oldItem));

            object newItem = e.NewItems.FirstOrDefault();
            int newIndex = Granular.Compatibility.Array.FindIndex(innerCollection, item => Granular.Compatibility.EqualityComparer.Default.Equals(item, newItem));

            if (oldIndex == -1 && newIndex == -1 || oldItem == newItem && oldIndex == newIndex)
            {
                return;
            }

            if (Granular.Compatibility.EqualityComparer.Default.Equals(oldItem, newItem))
            {
                CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Move(newItem, oldIndex, newIndex));
            }
            else if (oldIndex == newIndex)
            {
                CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Replace(oldItem, newItem, newIndex));
            }
            else
            {
                if (oldIndex != -1)
                {
                    CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Remove(oldItem, oldIndex));
                }

                if (newIndex != -1)
                {
                    CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Add(newItem, newIndex));
                }
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return Enumerable.Cast<object>(innerCollection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IEnumerable<object> TranslateCollection(IEnumerable<object> source, Func<object, bool> filterPredicate, Func<object, object> sortKeySelector, ListSortDirection sortDirection)
        {
            IEnumerable<object> collection = source;

            if (filterPredicate != null)
            {
                collection = collection.Where(filterPredicate);
            }

            if (sortKeySelector != null)
            {
                collection = sortDirection == ListSortDirection.Ascending ? collection.OrderBy(sortKeySelector) : collection.OrderByDescending(sortKeySelector);
            }

            return collection;
        }
    }
}
