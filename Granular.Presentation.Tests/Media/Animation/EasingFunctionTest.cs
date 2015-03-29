using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Granular.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Media.Animation
{
    [TestClass]
    public class EasingFunctionTest
    {
        [TestMethod]
        public void EasingFunctionModesTest()
        {
            PowerEase ease = new PowerEase { Power = 2 };

            ease.EasingMode = EasingMode.EaseIn;
            Assert.IsTrue(ease.Ease(0).IsClose(0));
            Assert.IsTrue(ease.Ease(0.3).IsClose(0.09));
            Assert.IsTrue(ease.Ease(0.7).IsClose(0.49));
            Assert.IsTrue(ease.Ease(1).IsClose(1));

            ease.EasingMode = EasingMode.EaseOut;
            Assert.IsTrue(ease.Ease(0).IsClose(0));
            Assert.IsTrue(ease.Ease(0.3).IsClose(0.51));
            Assert.IsTrue(ease.Ease(0.7).IsClose(0.91));
            Assert.IsTrue(ease.Ease(1).IsClose(1));

            ease.EasingMode = EasingMode.EaseInOut;
            Assert.IsTrue(ease.Ease(0).IsClose(0));
            Assert.IsTrue(ease.Ease(0.3).IsClose(0.18));
            Assert.IsTrue(ease.Ease(0.7).IsClose(0.82));
            Assert.IsTrue(ease.Ease(1).IsClose(1));
        }
    }
}
