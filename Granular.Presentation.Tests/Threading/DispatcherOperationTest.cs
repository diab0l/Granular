using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Threading
{
    [TestClass]
    public class DispatcherOperationTest
    {
        [TestMethod]
        public void InvokeParametersTest()
        {
            int value = 0;

            DispatcherOperation operation0 = new DispatcherOperation(() => value = 1, DispatcherPriority.Normal);
            operation0.Invoke();
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void InvokeResultTest()
        {
            DispatcherOperation operation0 = new DispatcherOperation(() => "operation-result", DispatcherPriority.Normal);
            operation0.Invoke();
            Assert.AreEqual("operation-result", operation0.Result);
        }
    }
}
