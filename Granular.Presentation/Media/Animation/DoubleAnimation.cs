using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    internal class DoubleAnimationOperations : IAnimationOperations<double?>
    {
        public static readonly DoubleAnimationOperations Default = new DoubleAnimationOperations();

        private DoubleAnimationOperations()
        {
            //
        }

        public double? Zero { get { return 0; } }

        public double? Add(double? value1, double? value2)
        {
            return value1.Value + value2.Value;
        }

        public double? Subtract(double? value1, double? value2)
        {
            return value1.Value - value2.Value;
        }

        public double? Scale(double? value, double factor)
        {
            return factor * value.Value;
        }

        public double? Interpolate(double? value1, double? value2, double progress)
        {
            return (1 - progress) * value1 + progress * value2;
        }
    }

    public class DoubleAnimation : TransitionAnimationTimeline<double?>
    {
        public DoubleAnimation() :
            base(DoubleAnimationOperations.Default, true)
        {
            //
        }
    }

    public class DoubleAnimationUsingKeyFrames : KeyFramesAnimationTimeline<double?>
    {
        public DoubleAnimationUsingKeyFrames() :
            base(DoubleAnimationOperations.Default, true)
        {
            //
        }
    }

    public class DiscreteDoubleKeyFrame : DiscreteKeyFrame<double?>
    {
        public DiscreteDoubleKeyFrame() :
            base(DoubleAnimationOperations.Default)
        {
            //
        }
    }

    public class LinearDoubleKeyFrame : LinearKeyFrame<double?>
    {
        public LinearDoubleKeyFrame() :
            base(DoubleAnimationOperations.Default)
        {
            //
        }
    }

    public class EasingDoubleKeyFrame : EasingKeyFrame<double?>
    {
        public EasingDoubleKeyFrame() :
            base(DoubleAnimationOperations.Default)
        {
            //
        }
    }
}
