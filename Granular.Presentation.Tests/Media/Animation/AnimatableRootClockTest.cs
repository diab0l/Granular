using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using Granular.Presentation.Media.Animation.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Media.Animation
{
    [TestClass]
    public class AnimatableRootClockTest
    {
        [TestMethod]
        public void AnimatableRootClockConnectionTest()
        {
            TestRootClock rootClock = new TestRootClock();

            FrameworkElement element = new FrameworkElement();
            element.SetAnimatableRootClock(new AnimatableRootClock(rootClock, false));

            Assert.AreEqual(0, rootClock.Clocks.Count());
            Assert.IsFalse(element.IsVisible);

            element.IsRootElement = true;
            Assert.IsTrue(element.IsVisible);

            DoubleAnimation animation = new DoubleAnimation { From = 0, To = 1 };

            element.BeginAnimation(FrameworkElement.WidthProperty, animation);
            Assert.AreEqual(1, rootClock.Clocks.Count());

            rootClock.Tick(TimeSpan.FromSeconds(0.1));
            Assert.AreEqual(0.1, element.Width);

            element.IsRootElement = false;
            Assert.IsFalse(element.IsVisible);
            Assert.AreEqual(0, rootClock.Clocks.Count());

            rootClock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(0.1, element.Width);

            element.IsRootElement = true;
            Assert.IsTrue(element.IsVisible);
            Assert.AreEqual(1, rootClock.Clocks.Count());
            Assert.AreEqual(0.2, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(0, rootClock.Clocks.Count());

            element.IsRootElement = false;
            element.IsRootElement = true;
            Assert.AreEqual(0, rootClock.Clocks.Count());
        }
    }
}
