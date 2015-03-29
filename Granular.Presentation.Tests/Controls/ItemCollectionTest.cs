using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Granular.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class ItemCollectionTest
    {
        [TestMethod]
        public void ItemCollectionDefaultViewTest()
        {
            ItemCollection itemCollection = new ItemCollection();

            NotifyCollectionChangedEventArgs lastChangeArgs = null;
            itemCollection.CollectionChanged += (sender, e) => lastChangeArgs = e;

            itemCollection.Add("default1");

            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastChangeArgs.Action);
            CollectionAssert.AreEqual(new[] { "default1" }, lastChangeArgs.NewItems.Cast<string>().ToArray());
        }

        [TestMethod]
        public void ItemCollectionItemsSourceTest()
        {
            ObservableCollection<string> collection = new ObservableCollection<string>(new[] { "collection1", "collection2", "collection3" });

            InnerCollectionView view = new InnerCollectionView();
            view.Add("view1");
            view.Add("view2");
            view.Add("view3");
            view.CurrentItemIndex = 1;

            ItemCollection itemCollection = new ItemCollection();

            NotifyCollectionChangedEventArgs lastChangeArgs = null;
            int currentChangedCount = 0;

            itemCollection.CollectionChanged += (sender, e) => lastChangeArgs = e;
            itemCollection.CurrentChanged += (sender, e) => currentChangedCount++;

            itemCollection.SetItemsSource(collection);
            Assert.AreEqual(-1, itemCollection.CurrentItemIndex);
            Assert.AreEqual(1, currentChangedCount);

            collection.Add("collection4");
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastChangeArgs.Action);
            CollectionAssert.AreEqual(new[] { "collection4" }, lastChangeArgs.NewItems.Cast<string>().ToArray());
            Assert.AreEqual(-1, itemCollection.CurrentItemIndex);
            Assert.AreEqual(1, currentChangedCount);

            itemCollection.CurrentItemIndex = 0;
            Assert.AreEqual(0, itemCollection.CurrentItemIndex);
            Assert.AreEqual(2, currentChangedCount);

            itemCollection.SetItemsSource(view);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastChangeArgs.Action);
            CollectionAssert.AreEqual(new[] { "collection1", "collection2", "collection3", "collection4" }, lastChangeArgs.OldItems.Cast<string>().ToArray());
            CollectionAssert.AreEqual(new[] { "view1", "view2", "view3" }, lastChangeArgs.NewItems.Cast<string>().ToArray());
            Assert.AreEqual(1, itemCollection.CurrentItemIndex);
            Assert.AreEqual(3, currentChangedCount);

            itemCollection.SetItemsSource(collection);
            lastChangeArgs = null;
            view.Add("view4");
            Assert.IsNull(lastChangeArgs);
        }
    }
}
