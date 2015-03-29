using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using Granular.Presentation.Media.Animation.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests
{
    [TestClass]
    public class AnimationExpressionTest
    {
        [TestMethod]
        public void ApplyAnimationClockTest()
        {
            ApplyAnimationClockTest(new DoubleAnimation { From = 10, To = 20 });
            ApplyAnimationClockTest(new DoubleAnimation { From = 10, By = 10 });
            ApplyAnimationClockTest(new DoubleAnimation { To = 20 });
            ApplyAnimationClockTest(new DoubleAnimation { By = 10 });
        }

        private void ApplyAnimationClockTest(DoubleAnimation animation)
        {
            FrameworkElement element = new FrameworkElement();

            TestRootClock rootClock = new TestRootClock();
            rootClock.Tick(TimeSpan.FromSeconds(0));

            AnimationTimelineClock clock = (AnimationTimelineClock)animation.CreateClock();
            clock.Begin(rootClock);

            element.Width = 10;
            Assert.AreEqual(10, element.Width);

            element.ApplyAnimationClock(FrameworkElement.WidthProperty, clock);
            Assert.AreEqual(10, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(0.5));
            Assert.AreEqual(15, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(20, element.Width);

            clock.Stop();

            element.Width = 30;
            Assert.AreEqual(30, element.Width);
        }

        [TestMethod]
        public void AnimationExpressionFillBehaviorHoldEndTest()
        {
            DoubleAnimation animation = new DoubleAnimation { From = 10, To = 20, FillBehavior = FillBehavior.HoldEnd };

            TestRootClock rootClock = new TestRootClock();
            rootClock.Tick(TimeSpan.FromSeconds(1));

            FrameworkElement element = new FrameworkElement();
            Assert.AreEqual(Double.NaN, element.Width);

            AnimationTimelineClock animationClock = (AnimationTimelineClock)animation.CreateClock();
            element.ApplyAnimationClock(FrameworkElement.WidthProperty, animationClock);
            animationClock.Begin(rootClock);

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(10, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(20, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(3));
            Assert.AreEqual(20, element.Width);
        }

        [TestMethod]
        public void AnimationExpressionFillBehaviorStopTest()
        {
            DoubleAnimation animation = new DoubleAnimation { From = 10, To = 20, FillBehavior = FillBehavior.Stop };

            TestRootClock rootClock = new TestRootClock();
            rootClock.Tick(TimeSpan.FromSeconds(1));

            FrameworkElement element = new FrameworkElement();
            Assert.AreEqual(Double.NaN, element.Width);

            AnimationTimelineClock animationClock = (AnimationTimelineClock)animation.CreateClock();
            element.ApplyAnimationClock(FrameworkElement.WidthProperty, animationClock);
            animationClock.Begin(rootClock);

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(10, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(Double.NaN, element.Width);
        }
    }
}
