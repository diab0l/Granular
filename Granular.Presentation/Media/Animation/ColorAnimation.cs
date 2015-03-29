using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media.Animation
{
    internal class ColorAnimationOperations : IAnimationOperations<Color>
    {
        public static readonly ColorAnimationOperations Default = new ColorAnimationOperations();

        private ColorAnimationOperations()
        {
            //
        }

        public Color Zero { get { return Color.FromUInt32(0); } }

        public Color Add(Color value1, Color value2)
        {
            return value1 + value2;
        }

        public Color Subtract(Color value1, Color value2)
        {
            return value1 - value2;
        }

        public Color Scale(Color value, double factor)
        {
            return factor * value;
        }

        public Color Interpolate(Color value1, Color value2, double progress)
        {
            return (1 - progress) * value1 + progress * value2;
        }
    }

    public class ColorAnimation : TransitionAnimationTimeline<Color>
    {
        public ColorAnimation() :
            base(ColorAnimationOperations.Default, true)
        {
            //
        }
    }

    public class ColorAnimationUsingKeyFrames : KeyFramesAnimationTimeline<Color>
    {
        public ColorAnimationUsingKeyFrames() :
            base(ColorAnimationOperations.Default, true)
        {
            //
        }
    }

    public class DiscreteColorKeyFrame : DiscreteKeyFrame<Color>
    {
        public DiscreteColorKeyFrame() :
            base(ColorAnimationOperations.Default)
        {
            //
        }
    }

    public class LinearColorKeyFrame : LinearKeyFrame<Color>
    {
        public LinearColorKeyFrame() :
            base(ColorAnimationOperations.Default)
        {
            //
        }
    }

    public class EasingColorKeyFrame : EasingKeyFrame<Color>
    {
        public EasingColorKeyFrame() :
            base(ColorAnimationOperations.Default)
        {
            //
        }
    }
}
