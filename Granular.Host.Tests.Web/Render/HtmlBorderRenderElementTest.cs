using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Granular.Host.Render;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Host.Tests.Web.Render
{
    [TestClass]
    public class HtmlBorderRenderElementTest
    {
        [TestMethod]
        public void HtmlBorderRenderElementBasicTest()
        {
            TestRenderQueue renderQueue = new TestRenderQueue();
            HtmlBorderRenderElement element = new HtmlBorderRenderElement(renderQueue, HtmlValueConverter.Default);

            renderQueue.Render();

            Assert.IsFalse(renderQueue.Items.Contains(element));

            element.BorderBrush = Brushes.Red;
            element.BorderThickness = new Thickness(1, 2, 3, 4);
            element.CornerRadius = new CornerRadius(10, 20, 30, 40);
            element.Bounds = new Rect(new Size(200, 100));

            CollectionAssert.AreEqual(new [] { element }, renderQueue.Items);

            renderQueue.Render();

            Assert.AreEqual("2px 3px 4px 1px", element.HtmlElement.Style["border-width"]);
            Assert.AreEqual("solid", element.HtmlElement.Style["border-style"]);
            Assert.AreEqual("rgb(255, 0, 0)", element.HtmlElement.Style["border-color"]);
            Assert.AreEqual("10px 20px 30px 40px", element.HtmlElement.Style["border-radius"]);
            Assert.AreEqual("0px", element.HtmlElement.Style["left"]);
            Assert.AreEqual("0px", element.HtmlElement.Style["top"]);
            Assert.AreEqual("196px", element.HtmlElement.Style["width"]);
            Assert.AreEqual("94px", element.HtmlElement.Style["height"]);
            Assert.AreEqual("absolute", element.HtmlElement.Style["position"]);
            Assert.AreEqual("2 3 4 1", element.HtmlElement.Style["border-image-slice"]);
        }
    }
}
