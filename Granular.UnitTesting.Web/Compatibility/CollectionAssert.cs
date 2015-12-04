using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class CollectionAssert
    {
        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            T[] expectedArray = expected.ToArray();
            T[] actualArray = actual.ToArray();

            if (expectedArray.Length != actualArray.Length)
            {
                throw new Granular.Exception("CollectionAssert.AreEqual failed. (Different number of elements.)");
            }

            for (int i = 0; i < expectedArray.Length; i++)
            {
                if (!Granular.Compatibility.EqualityComparer<T>.Default.Equals(expectedArray[i], actualArray[i]))
                {
                    throw new Granular.Exception("CollectionAssert.AreEqual failed. (Element at index {0} do not match.)", i);
                }
            }
        }

        public static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            T[] expectedArray = expected.ToArray();
            T[] actualArray = actual.ToArray();

            if (expectedArray.Length != actualArray.Length)
            {
                throw new Granular.Exception("CollectionAssert.AreEquivalent failed. The number of elements in the collections do not match. Expected:<{0}>. Actual:<{1}>.", expectedArray.Length, actualArray.Length);
            }

            for (int i = 0; i < expectedArray.Length; i++)
            {
                T item = expectedArray[i];

                int expectedOccurrences = expectedArray.Count(expectedItem => Granular.Compatibility.EqualityComparer<T>.Default.Equals(expectedItem, item));
                int actualOccurrences = actualArray.Count(actualItem => Granular.Compatibility.EqualityComparer<T>.Default.Equals(actualItem, item));

                if (expectedOccurrences != actualOccurrences)
                {
                    throw new Granular.Exception("CollectionAssert.AreEquivalent failed. The expected collection contains {0} occurrence(s) of <{1}>. The actual collection contains {2} occurrence(s).", expectedOccurrences, item, actualOccurrences);
                }
            }
        }
    }
}
