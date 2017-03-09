using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Granular.Presentation.Tests.Media;
using System.Windows;
using System.Windows.Media;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class ImageTest
    {
        [TestMethod]
        public void ImageStretchNoneTest()
        {
            Image image = CreateImage();
            TestImageRenderElement imageRenderElement = GetImageRenderElement(image);

            image.Stretch = System.Windows.Media.Stretch.None;

            image.Measure(Size.Infinity);
            Assert.AreEqual(new Size(200, 100), image.DesiredSize);

            image.Arrange(new Rect(200, 100));
            Assert.AreEqual(new Rect(200, 100), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 200));
            Assert.AreEqual(new Rect(100, 50, 200, 100), imageRenderElement.Bounds);
        }

        [TestMethod]
        public void ImageStretchFillTest()
        {
            Image image = CreateImage();
            TestImageRenderElement imageRenderElement = GetImageRenderElement(image);

            image.Stretch = System.Windows.Media.Stretch.Fill;
            image.StretchDirection = StretchDirection.Both;

            image.Measure(Size.Infinity);
            Assert.AreEqual(Size.Zero, image.DesiredSize);

            image.Arrange(new Rect(200, 100));
            Assert.AreEqual(new Rect(200, 100), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 200));
            Assert.AreEqual(new Rect(400, 200), imageRenderElement.Bounds);

            image.StretchDirection = StretchDirection.DownOnly;

            image.Arrange(new Rect(400, 200));
            Assert.AreEqual(new Rect(100, 50, 200, 100), imageRenderElement.Bounds);

            image.Arrange(new Rect(100, 50));
            Assert.AreEqual(new Rect(100, 50), imageRenderElement.Bounds);

            image.StretchDirection = StretchDirection.UpOnly;

            image.Arrange(new Rect(400, 200));
            Assert.AreEqual(new Rect(400, 200), imageRenderElement.Bounds);

            image.Arrange(new Rect(100, 50));
            Assert.AreEqual(new Rect(-50, -25, 200, 100), imageRenderElement.Bounds);
        }

        [TestMethod]
        public void ImageStretchUniformTest()
        {
            Image image = CreateImage();
            TestImageRenderElement imageRenderElement = GetImageRenderElement(image);

            image.Stretch = System.Windows.Media.Stretch.Uniform;
            image.StretchDirection = StretchDirection.Both;

            image.Measure(Size.Infinity);
            Assert.AreEqual(Size.Zero, image.DesiredSize);

            image.Arrange(new Rect(100, 200));
            Assert.AreEqual(new Rect(0, 75, 100, 50), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 800));
            Assert.AreEqual(new Rect(0, 300, 400, 200), imageRenderElement.Bounds);

            image.StretchDirection = StretchDirection.DownOnly;

            image.Arrange(new Rect(100, 200));
            Assert.AreEqual(new Rect(0, 75, 100, 50), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 800));
            Assert.AreEqual(new Rect(100, 350, 200, 100), imageRenderElement.Bounds);

            image.StretchDirection = StretchDirection.UpOnly;

            image.Arrange(new Rect(100, 200));
            Assert.AreEqual(new Rect(-50, 50, 200, 100), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 800));
            Assert.AreEqual(new Rect(0, 300, 400, 200), imageRenderElement.Bounds);
        }

        [TestMethod]
        public void ImageStretchUniformToFillTest()
        {
            Image image = CreateImage();
            TestImageRenderElement imageRenderElement = GetImageRenderElement(image);

            image.Stretch = System.Windows.Media.Stretch.UniformToFill;
            image.StretchDirection = StretchDirection.Both;

            image.Measure(Size.Infinity);
            Assert.AreEqual(Size.Zero, image.DesiredSize);

            image.Arrange(new Rect(100, 200));
            Assert.AreEqual(new Rect(-150, 0, 400, 200), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 800));
            Assert.AreEqual(new Rect(-600, 0, 1600, 800), imageRenderElement.Bounds);

            image.StretchDirection = StretchDirection.DownOnly;

            image.Arrange(new Rect(40, 80));
            Assert.AreEqual(new Rect(-60, 0, 160, 80), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 800));
            Assert.AreEqual(new Rect(100, 350, 200, 100), imageRenderElement.Bounds);

            image.StretchDirection = StretchDirection.UpOnly;

            image.Arrange(new Rect(40, 80));
            Assert.AreEqual(new Rect(-80, -10, 200, 100), imageRenderElement.Bounds);

            image.Arrange(new Rect(400, 800));
            Assert.AreEqual(new Rect(-600, 0, 1600, 800), imageRenderElement.Bounds);
        }

        private static Image CreateImage()
        {
            BitmapSource bitmapSource = BitmapSource.Create(null);
            ((TestRenderImageSource)bitmapSource.RenderImageSource).Size = new Size(200, 100);

            return new Image { Source = bitmapSource, IsRootElement = true };
        }

        private static TestImageRenderElement GetImageRenderElement(Image image)
        {
            return (TestImageRenderElement)((TestVisualRenderElement)image.GetRenderElement(TestRenderElementFactory.Default)).Content;
        }
    }
}
