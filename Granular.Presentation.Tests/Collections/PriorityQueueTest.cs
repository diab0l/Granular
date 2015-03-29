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
        private class CustomComparer<T> : IComparer<T>
        {
            private Func<T, T, int> comparer;

            public CustomComparer(Func<T, T, int> comparer)
            {
                this.comparer = comparer;
            }

            public int Compare(T x, T y)
            {
                return comparer(x, y);
            }
        }

        [TestMethod]
        public void PriorityQueueStabilityTest()
        {
            CustomComparer<int> comparer = new CustomComparer<int>((a, b) => Comparer<int>.Default.Compare(a / 10, b / 10));
            PriorityQueue<int, int> queue = new PriorityQueue<int, int>(comparer);
            List<int> list = new List<int>();
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                int n = random.Next(100);

                int j = 0;
                while (j < list.Count && comparer.Compare(list[j], n) != 1)
                {
                    j++;
                }

                list.Insert(j, n);
                queue.Enqueue(n, n);
            }

            for (int i = 0; i < list.Count; i++)
            {
                int n = queue.Dequeue();
                Assert.AreEqual(list[i], n);
            }
        }
    }
}
