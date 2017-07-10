using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Host.Tests.Web
{
    [TestClass]
    public class HtmlValueConverterTest
    {
        [TestMethod]
        public void HtmlValueConverterBasicTest()
        {
            HtmlValueConverter converter = new HtmlValueConverter();

            Assert.AreEqual("1.23px", converter.ToPixelString(1.234));
            Assert.AreEqual("123.46%", converter.ToPercentString(1.23456));
            Assert.AreEqual("1.23deg", converter.ToDegreesString(1.234));
            Assert.AreEqual("1.23px 5.68px", converter.ToPixelString(new Point(1.234, 5.678)));
            Assert.AreEqual("123.46% 567.89%", converter.ToPercentString(new Point(1.23456, 5.6789)));
            Assert.AreEqual("rgba(12, 34, 56, 0.25)", converter.ToColorString(Color.FromArgb(64, 12, 34, 56)));
            Assert.AreEqual("#0c2238", converter.ToColorString(Color.FromRgb(12, 34, 56)));
            Assert.AreEqual("1.23px", converter.ToPixelString(new Thickness(1.234)));
            Assert.AreEqual("2.35px 3.46px 4.57px 1.23px", converter.ToPixelString(new Thickness(1.234, 2.345, 3.456, 4.567)));
            Assert.AreEqual("2.35 3.46 4.57 1.23", converter.ToImplicitValueString(new Thickness(1.234, 2.345, 3.456, 4.567)));
            Assert.AreEqual("url(image-source)", converter.ToUrlString("image-source"));
            Assert.AreEqual("linear-gradient(135deg, #ff0000 0%, #0000ff 100%)", converter.ToLinearGradientString(new LinearGradientBrush(45, Colors.Red, Colors.Blue), new Rect(100, 100)));
            Assert.AreEqual("repeating-linear-gradient(135deg, #ff0000 0%, #0000ff 100%)", converter.ToLinearGradientString(new LinearGradientBrush(45, Colors.Red, Colors.Blue) { SpreadMethod = GradientSpreadMethod.Repeat }, new Rect(100, 100)));
            Assert.AreEqual("linear-gradient(135deg, #ff0000 0%, #0000ff 50%, #0000ff 50%, #ff0000 100%)", converter.ToLinearGradientString(new LinearGradientBrush(45, Colors.Red, Colors.Blue) { SpreadMethod = GradientSpreadMethod.Reflect }, new Rect(100, 100)));
            Assert.AreEqual("radial-gradient(ellipse 50% 50% at 50% 50%, #ff0000 0%, #0000ff 100%)", converter.ToRadialGradientString(new RadialGradientBrush(Colors.Red, Colors.Blue)));
            Assert.AreEqual("repeating-radial-gradient(ellipse 50% 50% at 50% 50%, #ff0000 0%, #0000ff 100%)", converter.ToRadialGradientString(new RadialGradientBrush(Colors.Red, Colors.Blue) { SpreadMethod = GradientSpreadMethod.Repeat }));
            Assert.AreEqual("#ff0000 0%, #0000ff 100%", converter.ToColorStopsString(new GradientStop[] { new GradientStop(Colors.Red, 0), new GradientStop(Colors.Blue, 1) }));
            Assert.AreEqual("#ff0000", converter.ToColorString(new SolidColorBrush(Colors.Red)));
            Assert.AreEqual("linear-gradient(135deg, #ff0000 0%, #0000ff 100%)", converter.ToImageString(new LinearGradientBrush(45, Colors.Red, Colors.Blue), new Rect(100, 100)));
            Assert.AreEqual("radial-gradient(ellipse 50% 50% at 50% 50%, #ff0000 0%, #0000ff 100%)", converter.ToImageString(new RadialGradientBrush(Colors.Red, Colors.Blue)));
            Assert.AreEqual("url(image-source)", converter.ToImageString(new ImageBrush() { ImageSource = "image-source" }));
        }

        [TestMethod]
        public void HtmlValueConverterLinearGradientTest()
        {
            HtmlValueConverter converter = new HtmlValueConverter();

            LinearGradientBrush brush = new LinearGradientBrush(new Point(0.1, 0.8), new Point(0.8, 0.1), Colors.Red, Colors.Blue);

            Assert.AreEqual("linear-gradient(45deg, #ff0000 15%, #0000ff 85%)", converter.ToLinearGradientString(brush, new Rect(100, 100)));
            Assert.AreEqual("linear-gradient(26.57deg, #ff0000 18%, #0000ff 88%)", converter.ToLinearGradientString(brush, new Rect(100, 200)));
            Assert.AreEqual("linear-gradient(45deg, #ff0000 15%, #0000ff 85%)", converter.ToLinearGradientString(brush, new Rect(10, 10, 80, 80)));
            Assert.AreEqual("linear-gradient(23.96deg, #ff0000 18.35%, #0000ff 88.35%)", converter.ToLinearGradientString(brush, new Rect(10, 10, 80, 180)));

            LinearGradientBrush absoluteBrush = new LinearGradientBrush(new Point(10, 80), new Point(80, 10), Colors.Red, Colors.Blue) { MappingMode = BrushMappingMode.Absolute };

            Assert.AreEqual("linear-gradient(45deg, #ff0000 15%, #0000ff 85%)", converter.ToLinearGradientString(absoluteBrush, new Rect(100, 100)));
            Assert.AreEqual("linear-gradient(45deg, #ff0000 43.33%, #0000ff 90%)", converter.ToLinearGradientString(absoluteBrush, new Rect(100, 200)));
            Assert.AreEqual("linear-gradient(45deg, #ff0000 6.25%, #0000ff 93.75%)", converter.ToLinearGradientString(absoluteBrush, new Rect(10, 10, 80, 80)));
            Assert.AreEqual("linear-gradient(45deg, #ff0000 42.31%, #0000ff 96.15%)", converter.ToLinearGradientString(absoluteBrush, new Rect(10, 10, 80, 180)));
        }
    }
}
