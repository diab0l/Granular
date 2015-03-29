using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class StoryboardTest
    {
        [TestMethod]
        public void StoryboardBasicTest()
        {
            DoubleAnimation widthAnimation = new DoubleAnimation { To = 100 };
            DoubleAnimation heightAnimation = new DoubleAnimation { From = 100 };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);
            storyboard.Children.Add(heightAnimation);

            FrameworkElement element = new FrameworkElement { Width = 0, Height = 0 };

            Storyboard.SetTarget(widthAnimation, element);
            Storyboard.SetTargetProperty(widthAnimation, PropertyPath.FromDependencyProperty(FrameworkElement.WidthProperty));

            Storyboard.SetTarget(heightAnimation, element);
            Storyboard.SetTargetProperty(heightAnimation, PropertyPath.FromDependencyProperty(FrameworkElement.HeightProperty));

            TestRootClock rootClock = new TestRootClock();
            element.SetAnimatableRootClock(new AnimatableRootClock(rootClock, true));
            storyboard.Begin(element);

            rootClock.Tick(TimeSpan.FromSeconds(0));
            Assert.AreEqual(0, element.Width);
            Assert.AreEqual(100, element.Height);

            rootClock.Tick(TimeSpan.FromSeconds(0.1));
            Assert.AreEqual(10, element.Width);
            Assert.AreEqual(90, element.Height);

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(100, element.Width);
            Assert.AreEqual(0, element.Height);

            storyboard.Seek(element, TimeSpan.FromSeconds(0.5));
            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(50, element.Width);
            Assert.AreEqual(50, element.Height);

            storyboard.Remove(element);
            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(0, element.Width);
            Assert.AreEqual(0, element.Height);
        }

        [TestMethod]
        public void StoryboardTargetTest()
        {
            XamlNamespaces namespaces = new XamlNamespaces("http://schemas.microsoft.com/winfx/2006/xaml/presentation");

            ColorAnimation colorAnimation = new ColorAnimation { From = Colors.Green, To = Colors.Blue };
            Storyboard.SetTargetProperty(colorAnimation, PropertyPath.Parse("(Control.Background).(SolidColorBrush.Color)", namespaces));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);

            TestRootClock rootClock = new TestRootClock();

            Control control = new Control();
            control.SetAnimatableRootClock(new AnimatableRootClock(rootClock, true));
            control.Background = new SolidColorBrush(Colors.Red);

            storyboard.Begin(control);

            rootClock.Tick(TimeSpan.FromSeconds(0));
            Assert.AreEqual(Colors.Green, ((SolidColorBrush)control.Background).Color);

            rootClock.Tick(TimeSpan.FromSeconds(0.5));
            Assert.IsTrue(Color.FromArgb(255, 0, (byte)(Colors.Green.G / 2), (byte)(Colors.Blue.B / 2)).IsClose(((SolidColorBrush)control.Background).Color));

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(Colors.Blue, ((SolidColorBrush)control.Background).Color);
        }
    }
}
