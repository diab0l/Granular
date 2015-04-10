using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media.Animation
{
    internal class ThicknessAnimationOperations : IAnimationOperations<Thickness>
    {
        public static readonly ThicknessAnimationOperations Default = new ThicknessAnimationOperations();

        private ThicknessAnimationOperations()
        {
            //
        }

        public Thickness Zero { get { return Thickness.Zero; } }

        public Thickness Add(Thickness value1, Thickness value2)
        {
            return value1 + value2;
        }

        public Thickness Subtract(Thickness value1, Thickness value2)
        {
            return value1 - value2;
        }

        public Thickness Scale(Thickness value, double factor)
        {
            return factor * value;
        }

        public Thickness Interpolate(Thickness value1, Thickness value2, double progress)
        {
            return (1 - progress) * value1 + progress * value2;
        }
    }

    public class ThicknessAnimation : TransitionAnimationTimeline<Thickness>
    {
        public ThicknessAnimation() :
            base(ThicknessAnimationOperations.Default, true)
        {
            //
        }
    }

    public class ThicknessAnimationUsingKeyFrames : KeyFramesAnimationTimeline<Thickness>
    {
        public ThicknessAnimationUsingKeyFrames() :
            base(ThicknessAnimationOperations.Default, true)
        {
            //
        }
    }

    public class DiscreteThicknessKeyFrame : DiscreteKeyFrame<Thickness>
    {
        public DiscreteThicknessKeyFrame() :
            base(ThicknessAnimationOperations.Default)
        {
            //
        }
    }

    public class LinearThicknessKeyFrame : LinearKeyFrame<Thickness>
    {
        public LinearThicknessKeyFrame() :
            base(ThicknessAnimationOperations.Default)
        {
            //
        }
    }

    public class EasingThicknessKeyFrame : EasingKeyFrame<Thickness>
    {
        public EasingThicknessKeyFrame() :
            base(ThicknessAnimationOperations.Default)
        {
            //
        }
    }
}
