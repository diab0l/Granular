using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Data
{
    [TestClass]
    public class CollectionViewTest
    {
        [TestMethod]
        public void CollectionViewChangeTest()
        {
            ObservableCollection<int> collection = new ObservableCollection<int>(new[] { 0, 1, 9, 8, 2, 3, 7, 6, 4, 5 });
            CollectionView collectionView = new CollectionView(collection);

            NotifyCollectionChangedEventArgs lastChangeArgs = null;
            int currentChangedCount = 0;

            collectionView.CollectionChanged += (sender, e) => lastChangeArgs = e;
            collectionView.CurrentChanged += (sender, e) => currentChangedCount++;

            collectionView.CurrentItem = 3;
            Assert.AreEqual(3, collectionView.CurrentItem);
            Assert.AreEqual(5, collectionView.CurrentItemIndex);
            Assert.AreEqual(1, currentChangedCount);

            collectionView.CurrentItemIndex = 3;
            Assert.AreEqual(8, collectionView.CurrentItem);
            Assert.AreEqual(3, collectionView.CurrentItemIndex);
            Assert.AreEqual(2, currentChangedCount);

            collection.Add(10);
            Assert.AreEqual(8, collectionView.CurrentItem);
            Assert.AreEqual(3, collectionView.CurrentItemIndex);
            Assert.AreEqual(2, currentChangedCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastChangeArgs.Action);
            Assert.AreEqual(10, lastChangeArgs.NewStartingIndex);
            CollectionAssert.AreEqual(new [] { 10 }, lastChangeArgs.NewItems.Cast<int>().ToArray());
            CollectionAssert.AreEqual(new [] { 0, 1, 9, 8, 2, 3, 7, 6, 4, 5, 10 }, collectionView.Cast<int>().ToArray());

            collection.Insert(0, 11);
            Assert.AreEqual(8, collectionView.CurrentItem);
            Assert.AreEqual(4, collectionView.CurrentItemIndex);
            Assert.AreEqual(3, currentChangedCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastChangeArgs.Action);
            Assert.AreEqual(0, lastChangeArgs.NewStartingIndex);
            CollectionAssert.AreEqual(new[] { 11 }, lastChangeArgs.NewItems.Cast<int>().ToArray());
            CollectionAssert.AreEqual(new[] { 11, 0, 1, 9, 8, 2, 3, 7, 6, 4, 5, 10 }, collectionView.Cast<int>().ToArray());

            collection.RemoveAt(4);
            Assert.AreEqual(2, collectionView.CurrentItem);
            Assert.AreEqual(4, collectionView.CurrentItemIndex);
            Assert.AreEqual(4, currentChangedCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastChangeArgs.Action);
            Assert.AreEqual(4, lastChangeArgs.OldStartingIndex);
            CollectionAssert.AreEqual(new[] { 8 }, lastChangeArgs.OldItems.Cast<int>().ToArray());
            CollectionAssert.AreEqual(new[] { 11, 0, 1, 9, 2, 3, 7, 6, 4, 5, 10 }, collectionView.Cast<int>().ToArray());

            collection[4] = 12;
            Assert.AreEqual(12, collectionView.CurrentItem);
            Assert.AreEqual(4, collectionView.CurrentItemIndex);
            Assert.AreEqual(5, currentChangedCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Replace, lastChangeArgs.Action);
            Assert.AreEqual(4, lastChangeArgs.NewStartingIndex);
            CollectionAssert.AreEqual(new[] { 12 }, lastChangeArgs.NewItems.Cast<int>().ToArray());
            CollectionAssert.AreEqual(new[] { 2 }, lastChangeArgs.OldItems.Cast<int>().ToArray());
            CollectionAssert.AreEqual(new[] { 11, 0, 1, 9, 12, 3, 7, 6, 4, 5, 10 }, collectionView.Cast<int>().ToArray());

            collectionView.CurrentItemIndex = 20;
            Assert.IsNull(collectionView.CurrentItem);
            Assert.AreEqual(11, collectionView.CurrentItemIndex);
            Assert.AreEqual(6, currentChangedCount);

            collection.Add(13);
            Assert.IsNull(collectionView.CurrentItem);
            Assert.AreEqual(12, collectionView.CurrentItemIndex);
            Assert.AreEqual(7, currentChangedCount);
            CollectionAssert.AreEqual(new[] { 11, 0, 1, 9, 12, 3, 7, 6, 4, 5, 10, 13 }, collectionView.Cast<int>().ToArray());

            collection.RemoveAt(0);
            Assert.IsNull(collectionView.CurrentItem);
            Assert.AreEqual(11, collectionView.CurrentItemIndex);
            Assert.AreEqual(8, currentChangedCount);
            CollectionAssert.AreEqual(new[] { 0, 1, 9, 12, 3, 7, 6, 4, 5, 10, 13 }, collectionView.Cast<int>().ToArray());
        }

        [TestMethod]
        public void CollectionViewSortTest()
        {
            ObservableCollection<int> collection = new ObservableCollection<int>(new[] { 0, 1, 9, 8, 2, 3, 7, 6, 4, 5 });
            CollectionView collectionView = new CollectionView(collection);

            NotifyCollectionChangedEventArgs lastChangeArgs = null;
            int currentChangedCount = 0;

            collectionView.CollectionChanged += (sender, e) => lastChangeArgs = e;
            collectionView.CurrentChanged += (sender, e) => currentChangedCount++;

            collectionView.CurrentItem = 3;
            Assert.AreEqual(3, collectionView.CurrentItem);
            Assert.AreEqual(5, collectionView.CurrentItemIndex);
            Assert.AreEqual(1, currentChangedCount);

            collectionView.SortKeySelector = item => item;
            Assert.AreEqual(3, collectionView.CurrentItem);
            Assert.AreEqual(3, collectionView.CurrentItemIndex);
            Assert.AreEqual(2, currentChangedCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastChangeArgs.Action);
            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, collectionView.Cast<int>().ToArray());

            collectionView.SortDirection = ListSortDirection.Descending;
            Assert.AreEqual(3, collectionView.CurrentItem);
            Assert.AreEqual(6, collectionView.CurrentItemIndex);
            Assert.AreEqual(3, currentChangedCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastChangeArgs.Action);
            CollectionAssert.AreEqual(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, collectionView.Cast<int>().ToArray());
        }

        [TestMethod]
        public void CollectionViewFilterTest()
        {
            ObservableCollection<int> collection = new ObservableCollection<int>(new[] { 0, 1, 9, 8, 2, 3, 7, 6, 4, 5 });
            CollectionView collectionView = new CollectionView(collection);

            NotifyCollectionChangedEventArgs lastChangeArgs = null;
            int currentChangedCount = 0;

            collectionView.CollectionChanged += (sender, e) => lastChangeArgs = e;
            collectionView.CurrentChanged += (sender, e) => currentChangedCount++;

            collectionView.CurrentItem = 2;
            Assert.AreEqual(2, collectionView.CurrentItem);
            Assert.AreEqual(4, collectionView.CurrentItemIndex);
            Assert.AreEqual(1, currentChangedCount);

            collectionView.FilterPredicate = item => (int)item % 2 == 0;
            Assert.AreEqual(2, collectionView.CurrentItem);
            Assert.AreEqual(2, collectionView.CurrentItemIndex);
            Assert.AreEqual(2, currentChangedCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastChangeArgs.Action);
            CollectionAssert.AreEqual(new[] { 0, 8, 2, 6, 4 }, collectionView.Cast<int>().ToArray());
        }
    }
}
