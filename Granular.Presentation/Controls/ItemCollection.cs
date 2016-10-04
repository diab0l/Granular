using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows.Controls
{
    // ICollectionView adapter for a replaceable external IEnumerable source
    public class ItemCollection : ICollectionView, IList<object>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler CurrentChanged;

        public IEnumerable SourceCollection { get { return delegateView.SourceCollection; } }

        public object CurrentItem
        {
            get { return delegateView.CurrentItem; }
            set { delegateView.CurrentItem = value; }
        }

        public int CurrentItemIndex
        {
            get { return delegateView.CurrentItemIndex; }
            set { delegateView.CurrentItemIndex = value; }
        }

        public bool CanFilter { get { return delegateView.CanFilter; } }

        public Func<object, bool> FilterPredicate
        {
            get { return delegateView.FilterPredicate; }
            set { delegateView.FilterPredicate = value; }
        }

        public bool CanSort { get { return delegateView.CanSort; } }

        public Func<object, object> SortKeySelector
        {
            get { return delegateView.SortKeySelector; }
            set { delegateView.SortKeySelector = value; }
        }

        public ListSortDirection SortDirection
        {
            get { return delegateView.SortDirection; }
            set { delegateView.SortDirection = value; }
        }

        public object this[int index]
        {
            get { return delegateView.ElementAt(index); }
            set
            {
                VerifyDefaultView();
                defaultView[index] = value;
            }
        }

        public int Count { get { return delegateView.Count(); } }

        private InnerCollectionView defaultView; // a local view when there is no source
        private CollectionView innerView; // a local adapter for the source (when it doesn't implement ICollectionView)

        private ICollectionView delegateView; // the source if it implements ICollectionView, or the local innerView if it doesn't

        public ItemCollection()
        {
            defaultView = new InnerCollectionView();
            SetDelegateView(defaultView);
        }

        public void SetItemsSource(IEnumerable source)
        {
            if (innerView != null)
            {
                if (innerView.SourceCollection == source)
                {
                    return;
                }

                innerView.Dispose();
                innerView = null;
            }

            if (source is ICollectionView)
            {
                SetDelegateView((ICollectionView)source);
            }
            else
            {
                innerView = new CollectionView(source);
                SetDelegateView(innerView);
            }
        }

        public void ClearItemsSource()
        {
            if (innerView != null)
            {
                innerView.Dispose();
                innerView = null;
            }

            SetDelegateView(defaultView);
        }

        private void SetDelegateView(ICollectionView collectionView)
        {
            if (delegateView == collectionView)
            {
                return;
            }

            IEnumerable<object> oldItems;
            if (delegateView != null)
            {
                delegateView.CollectionChanged -= OnDelegateViewCollectionChanged;
                delegateView.CurrentChanged -= OnDelegateViewCurrentChanged;
                oldItems = delegateView;
            }
            else
            {
                oldItems = new object[0];
            }

            delegateView = collectionView ?? CollectionView.Empty;

            delegateView.CollectionChanged += OnDelegateViewCollectionChanged;
            delegateView.CurrentChanged += OnDelegateViewCurrentChanged;

            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Reset(oldItems, delegateView));
            CurrentChanged.Raise(this);
        }

        public void OnDelegateViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged.Raise(this, e);
        }

        public void OnDelegateViewCurrentChanged(object sender, EventArgs e)
        {
            CurrentChanged.Raise(this, e);
        }

        public void Add(object value)
        {
            VerifyDefaultView();
            defaultView.Add(value);
        }

        public void Clear()
        {
            VerifyDefaultView();
            defaultView.Clear();
        }

        public bool Contains(object value)
        {
            VerifyDefaultView();
            return defaultView.Contains(value);
        }

        public int IndexOf(object value)
        {
            return Granular.Compatibility.Array.IndexOf(delegateView.ToArray(), value);
        }

        public void Insert(int index, object value)
        {
            VerifyDefaultView();
            defaultView.Insert(index, value);
        }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(object value)
        {
            VerifyDefaultView();
            return defaultView.Remove(value);
        }

        public void RemoveAt(int index)
        {
            VerifyDefaultView();
            defaultView.RemoveAt(index);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            delegateView.ToArray().CopyTo(array, arrayIndex);
        }

        private void VerifyDefaultView()
        {
            if (delegateView != defaultView)
            {
                throw new Granular.Exception("Can't change ItemCollection while ItemSource is set");
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return delegateView.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
