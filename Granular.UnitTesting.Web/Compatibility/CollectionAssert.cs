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
    }
}
