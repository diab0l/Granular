using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Granular.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class DoubleAnimationTest
    {
        [TestMethod]
        public void DoubleAnimationTransitionTest()
        {
            DoubleAnimationBasicTest(new DoubleAnimation { From = 20 }, 10, 30, 20, 25, 30);
            DoubleAnimationBasicTest(new DoubleAnimation { From = 20, By = 10 }, 10, 30, 20, 25, 30);
            DoubleAnimationBasicTest(new DoubleAnimation { From = 20, To = 30 }, 10, 30, 20, 25, 30);
            DoubleAnimationBasicTest(new DoubleAnimation { By = 10 }, 10, 30, 10, 15, 20);
            DoubleAnimationBasicTest(new DoubleAnimation { To = 20 }, 10, 30, 10, 15, 20);
        }

        [TestMethod]
        public void DoubleAnimationKeyFrameTest()
        {
            DoubleAnimationUsingKeyFrames animation;

            animation = new DoubleAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 2 });
            animation.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 4 });
            animation.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.8)), Value = 6 });

            DoubleAnimationBasicTest(animation, 10, 20, 10, 4, 6);

            animation = new DoubleAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = KeyTime.FromPercent(0.2), Value = 4 });
            animation.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = KeyTime.FromPercent(0.8), Value = 6 });

            DoubleAnimationBasicTest(animation, 10, 20, 10, 4, 6);

            animation = new DoubleAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = 4 });
            animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.8)), Value = 6 });

            DoubleAnimationBasicTest(animation, 10, 20, 10, 5, 6);

            animation = new DoubleAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromPercent(0.2), Value = 2 });
            animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromPercent(0.2), Value = 4 });
            animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromPercent(0.8), Value = 6 });

            DoubleAnimationBasicTest(animation, 10, 20, 10, 5, 6);
        }

        private void DoubleAnimationBasicTest(AnimationTimeline animation, double defaultOriginValue, double defaultDestinationValue, double expectedStartValue, double expectedMiddleValue, double expectedEndValue)
        {
            AnimationTimelineClock clock = (AnimationTimelineClock)animation.CreateClock();

            TestRootClock rootClock = new TestRootClock();

            clock.Begin(rootClock);

            rootClock.Tick(TimeSpan.Zero);
            Assert.AreEqual(expectedStartValue, (double)animation.GetCurrentValue(defaultOriginValue, defaultDestinationValue, clock));

            rootClock.Tick(animation.Duration.TimeSpan.Scale(0.5));
            Assert.AreEqual(expectedMiddleValue, (double)animation.GetCurrentValue(defaultOriginValue, defaultDestinationValue, clock));

            rootClock.Tick(animation.Duration.TimeSpan);
            Assert.AreEqual(expectedEndValue, (double)animation.GetCurrentValue(defaultOriginValue, defaultDestinationValue, clock));
        }
    }
}
