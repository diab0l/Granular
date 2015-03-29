using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Granular.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class ColorAnimationTest
    {
        [TestMethod]
        public void ColorAnimationTransitionTest()
        {
            ColorAnimationBasicTest(new ColorAnimation { From = Color.FromUInt32(150) }, Color.FromUInt32(50), Color.FromUInt32(250), Color.FromUInt32(150), Color.FromUInt32(200), Color.FromUInt32(250));
            ColorAnimationBasicTest(new ColorAnimation { From = Color.FromUInt32(150), By = Color.FromUInt32(100) }, Color.FromUInt32(50), Color.FromUInt32(250), Color.FromUInt32(150), Color.FromUInt32(200), Color.FromUInt32(250));
            ColorAnimationBasicTest(new ColorAnimation { From = Color.FromUInt32(150), To = Color.FromUInt32(250) }, Color.FromUInt32(50), Color.FromUInt32(250), Color.FromUInt32(150), Color.FromUInt32(200), Color.FromUInt32(250));
            ColorAnimationBasicTest(new ColorAnimation { By = Color.FromUInt32(100) }, Color.FromUInt32(50), Color.FromUInt32(250), Color.FromUInt32(50), Color.FromUInt32(100), Color.FromUInt32(150));
            ColorAnimationBasicTest(new ColorAnimation { To = Color.FromUInt32(150) }, Color.FromUInt32(50), Color.FromUInt32(250), Color.FromUInt32(50), Color.FromUInt32(100), Color.FromUInt32(150));
        }

        [TestMethod]
        public void ColorAnimationKeyFrameTest()
        {
            ColorAnimationUsingKeyFrames animation;

            animation = new ColorAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new DiscreteColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = Color.FromUInt32(20) });
            animation.KeyFrames.Add(new DiscreteColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = Color.FromUInt32(40) });
            animation.KeyFrames.Add(new DiscreteColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.8)), Value = Color.FromUInt32(60) });

            ColorAnimationBasicTest(animation, Color.FromUInt32(50), Color.FromUInt32(150), Color.FromUInt32(50), Color.FromUInt32(40), Color.FromUInt32(60));

            animation = new ColorAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new DiscreteColorKeyFrame { KeyTime = KeyTime.FromPercent(0.2), Value = Color.FromUInt32(40) });
            animation.KeyFrames.Add(new DiscreteColorKeyFrame { KeyTime = KeyTime.FromPercent(0.8), Value = Color.FromUInt32(60) });

            ColorAnimationBasicTest(animation, Color.FromUInt32(50), Color.FromUInt32(150), Color.FromUInt32(50), Color.FromUInt32(40), Color.FromUInt32(60));

            animation = new ColorAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new LinearColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), Value = Color.FromUInt32(40) });
            animation.KeyFrames.Add(new LinearColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.8)), Value = Color.FromUInt32(60) });

            ColorAnimationBasicTest(animation, Color.FromUInt32(50), Color.FromUInt32(150), Color.FromUInt32(50), Color.FromUInt32(50), Color.FromUInt32(60));

            animation = new ColorAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new LinearColorKeyFrame { KeyTime = KeyTime.FromPercent(0.2), Value = Color.FromUInt32(20) });
            animation.KeyFrames.Add(new LinearColorKeyFrame { KeyTime = KeyTime.FromPercent(0.2), Value = Color.FromUInt32(40) });
            animation.KeyFrames.Add(new LinearColorKeyFrame { KeyTime = KeyTime.FromPercent(0.8), Value = Color.FromUInt32(60) });

            ColorAnimationBasicTest(animation, Color.FromUInt32(50), Color.FromUInt32(150), Color.FromUInt32(50), Color.FromUInt32(50), Color.FromUInt32(60));
        }

        private void ColorAnimationBasicTest(AnimationTimeline animation, Color defaultOriginValue, Color defaultDestinationValue, Color expectedStartValue, Color expectedMiddleValue, Color expectedEndValue)
        {
            AnimationTimelineClock clock = (AnimationTimelineClock)animation.CreateClock();

            TestRootClock rootClock = new TestRootClock();

            clock.Begin(rootClock);

            rootClock.Tick(TimeSpan.Zero);
            Assert.AreEqual(expectedStartValue, (Color)animation.GetCurrentValue(defaultOriginValue, defaultDestinationValue, clock));

            rootClock.Tick(animation.Duration.TimeSpan.Scale(0.5));
            Assert.AreEqual(expectedMiddleValue, (Color)animation.GetCurrentValue(defaultOriginValue, defaultDestinationValue, clock));

            rootClock.Tick(animation.Duration.TimeSpan);
            Assert.AreEqual(expectedEndValue, (Color)animation.GetCurrentValue(defaultOriginValue, defaultDestinationValue, clock));
        }
    }
}
