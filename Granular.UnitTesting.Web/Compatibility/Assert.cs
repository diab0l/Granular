using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if (!Granular.Compatibility.EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new Granular.Exception(String.Format("Assert.AreEqual failed. Expected:<{0}>. Actual:<{1}>.", expected, actual));
            }
        }

        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            if (Granular.Compatibility.EqualityComparer<T>.Default.Equals(notExpected, actual))
            {
                throw new Granular.Exception(String.Format("Assert.AreNotEqual failed. Expected any value except:<{0}>. Actual:<{1}>.", notExpected, actual));
            }
        }

        public static void Fail()
        {
            throw new Granular.Exception("Assert.Fail failed.");
        }

        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                throw new Granular.Exception("Assert.IsTrue failed.");
            }
        }

        public static void IsFalse(bool condition)
        {
            if (condition)
            {
                throw new Granular.Exception("Assert.IsFalse failed.");
            }
        }

        public static void IsNull(object value)
        {
            if (value != null)
            {
                throw new Granular.Exception("Assert.IsNull failed.");
            }
        }

        public static void IsNotNull(object value)
        {
            if (value == null)
            {
                throw new Granular.Exception("Assert.IsNotNull failed.");
            }
        }
    }
}
