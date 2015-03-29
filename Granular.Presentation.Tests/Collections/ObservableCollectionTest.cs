using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Collections
{
    [TestClass]
    public class ObservableCollectionTest
    {
        [TestMethod]
        public void NotifyCollectionChangedTest()
        {
            int countChanged = 0;
            NotifyCollectionChangedEventArgs lastArgs = null;

            ObservableCollection<string> collection = new ObservableCollection<string>();
            collection.PropertyChanged += (sender, e) => countChanged += e.PropertyName == "Count" ? 1 : 0;
            collection.CollectionChanged += (sender, e) => lastArgs = e;

            collection.Add("item1");
            collection.Add("item2");
            collection.Add("item3");
            collection.Add("item4");
            collection.Add("item5");

            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastArgs.Action);
            Assert.AreEqual(4, lastArgs.NewStartingIndex);
            Assert.AreEqual(-1, lastArgs.OldStartingIndex);
            CollectionAssert.AreEqual(new[] { "item5" }, lastArgs.NewItems.ToArray());
            Assert.IsFalse(lastArgs.OldItems.Any());
            Assert.AreEqual(5, countChanged);

            collection.Remove("item3");
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastArgs.Action);
            Assert.AreEqual(-1, lastArgs.NewStartingIndex);
            Assert.AreEqual(2, lastArgs.OldStartingIndex);
            CollectionAssert.AreEqual(new[] { "item3" }, lastArgs.OldItems.ToArray());
            Assert.IsFalse(lastArgs.NewItems.Any());
            Assert.AreEqual(6, countChanged);

            collection.RemoveAt(3);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastArgs.Action);
            Assert.AreEqual(-1, lastArgs.NewStartingIndex);
            Assert.AreEqual(3, lastArgs.OldStartingIndex);
            Assert.IsFalse(lastArgs.NewItems.Any());
            CollectionAssert.AreEqual(new[] { "item5" }, lastArgs.OldItems.ToArray());
            Assert.AreEqual(7, countChanged);

            collection[1] = "item6";
            Assert.AreEqual(NotifyCollectionChangedAction.Replace, lastArgs.Action);
            Assert.AreEqual(1, lastArgs.NewStartingIndex);
            Assert.AreEqual(1, lastArgs.OldStartingIndex);
            CollectionAssert.AreEqual(new[] { "item6" }, lastArgs.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { "item2" }, lastArgs.OldItems.ToArray());
            Assert.AreEqual(7, countChanged);

            collection.Clear();
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastArgs.Action);
            Assert.AreEqual(-1, lastArgs.NewStartingIndex);
            Assert.AreEqual(0, lastArgs.OldStartingIndex);
            Assert.IsFalse(lastArgs.NewItems.Any());
            CollectionAssert.AreEqual(new[] { "item1", "item6", "item4" }, lastArgs.OldItems.ToArray());
            Assert.AreEqual(8, countChanged);
        }
    }
}
