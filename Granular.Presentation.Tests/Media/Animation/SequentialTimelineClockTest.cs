using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media.Animation;
using System.Windows;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class SequentialTimelineClockTest
    {
        [TestMethod]
        public void SequentialTimelineClockStoryboardTest()
        {
            DoubleAnimation animation1 = new DoubleAnimation { To = 100, Duration = Duration.FromTimeSpan(TimeSpan.FromSeconds(1)) };
            DoubleAnimation animation2 = new DoubleAnimation { To = 200, Duration = Duration.FromTimeSpan(TimeSpan.FromSeconds(0)) };
            DoubleAnimation animation3 = new DoubleAnimation { To = 300, Duration = Duration.FromTimeSpan(TimeSpan.FromSeconds(1)) };

            SequentialTimeline sequentialTimeline = new SequentialTimeline();
            sequentialTimeline.Children.Add(animation1);
            sequentialTimeline.Children.Add(animation2);
            sequentialTimeline.Children.Add(animation3);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(sequentialTimeline);

            FrameworkElement element = new FrameworkElement { Width = 0, Height = 0 };

            Storyboard.SetTarget(animation1, element);
            Storyboard.SetTargetProperty(animation1, PropertyPath.FromDependencyProperty(FrameworkElement.WidthProperty));

            Storyboard.SetTarget(animation2, element);
            Storyboard.SetTargetProperty(animation2, PropertyPath.FromDependencyProperty(FrameworkElement.WidthProperty));

            Storyboard.SetTarget(animation3, element);
            Storyboard.SetTargetProperty(animation3, PropertyPath.FromDependencyProperty(FrameworkElement.WidthProperty));

            TestRootClock rootClock = new TestRootClock();
            element.SetAnimatableRootClock(new AnimatableRootClock(rootClock, true));
            storyboard.Begin(element);

            rootClock.Tick(TimeSpan.FromSeconds(0));
            Assert.AreEqual(0, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(0.9));
            Assert.AreEqual(90, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(200, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(1.5));
            Assert.AreEqual(250, element.Width);

            rootClock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(300, element.Width);
        }
    }
}
