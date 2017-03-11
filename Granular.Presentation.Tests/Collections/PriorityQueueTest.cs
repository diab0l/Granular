using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Collections
{
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void PriorityQueueBasicTest()
        {
            PriorityQueue<string> queue = new PriorityQueue<string>(6);
            queue.Enqueue(5, "value1a");
            queue.Enqueue(3, "value2a");
            queue.Enqueue(2, "value3a");
            queue.Enqueue(4, "value4a");
            queue.Enqueue(1, "value5a");

            queue.Enqueue(5, "value1b");
            queue.Enqueue(3, "value2b");
            queue.Enqueue(2, "value3b");
            queue.Enqueue(4, "value4b");
            queue.Enqueue(1, "value5b");

            Assert.AreEqual(10, queue.Count);

            Assert.AreEqual("value1a", queue.Dequeue());
            Assert.AreEqual("value1b", queue.Dequeue());
            Assert.AreEqual("value4a", queue.Dequeue());
            Assert.AreEqual("value4b", queue.Dequeue());
            Assert.AreEqual("value2a", queue.Dequeue());
            Assert.AreEqual("value2b", queue.Dequeue());
            Assert.AreEqual("value3a", queue.Dequeue());
            Assert.AreEqual("value3b", queue.Dequeue());
            Assert.AreEqual("value5a", queue.Dequeue());
            Assert.AreEqual("value5b", queue.Dequeue());

            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        public void PriorityQueueStabilityTest()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>(10);
            List<int> list = new List<int>();
            Random random = new Random(1021);

            for (int i = 0; i < 100; i++)
            {
                int n = random.Next(100);

                int j = 0;
                while (j < list.Count && list[j] / 10 >= n / 10)
                {
                    j++;
                }

                list.Insert(j, n);
                queue.Enqueue(n / 10, n);
            }

            for (int i = 0; i < list.Count; i++)
            {
                int n = queue.Dequeue();
                Assert.AreEqual(list[i], n);
            }
        }
    }
}
