using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Markup
{
    public class UriContextTestElement : IUriContext
    {
        public Uri BaseUri { get; set; }
        public UriContextTestElement Child { get; set; }
    }

    [TestClass]
    public class UriContextTest
    {
        [TestMethod]
        public void SetBaseUriTest()
        {
            string text = @"
            <UriContextTestElement xmlns='clr-namespace:Granular.Presentation.Tests.Markup;assembly=Granular.Presentation.Tests'>
                <UriContextTestElement.Child>
                    <UriContextTestElement Child='{UriContextTestElement Child={UriContextTestElement}}'/>
                </UriContextTestElement.Child>
            </UriContextTestElement>";

            Uri sourceUri = new Uri("test:///uri");
            XamlElement rootElement = XamlParser.Parse(text, sourceUri);

            Assert.AreEqual(rootElement.SourceUri, sourceUri);

            object root = XamlLoader.Load(rootElement);

            Assert.IsTrue(root is UriContextTestElement);

            Assert.AreEqual(((UriContextTestElement)root).BaseUri, sourceUri);
            Assert.AreEqual(((UriContextTestElement)root).Child.BaseUri, sourceUri);
            Assert.AreEqual(((UriContextTestElement)root).Child.Child.BaseUri, sourceUri);
            Assert.AreEqual(((UriContextTestElement)root).Child.Child.Child.BaseUri, sourceUri);
        }
    }
}
