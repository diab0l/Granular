using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media.Animation
{
    internal class RectAnimationOperations : IAnimationOperations<Rect>
    {
        public static readonly RectAnimationOperations Default = new RectAnimationOperations();

        private RectAnimationOperations()
        {
            //
        }

        public Rect Zero { get { return Rect.Zero; } }

        public Rect Add(Rect value1, Rect value2)
        {
            return new Rect(value1.Location + value2.Location, value1.Size + value2.Size);
        }

        public Rect Subtract(Rect value1, Rect value2)
        {
            return new Rect(value1.Location - value2.Location, value1.Size - value2.Size);
        }

        public Rect Scale(Rect value, double factor)
        {
            return new Rect(factor * value.Location, factor * value.Size);
        }

        public Rect Interpolate(Rect value1, Rect value2, double progress)
        {
            return new Rect((1 - progress) * value1.Location + progress * value2.Location, (1 - progress) * value1.Size + progress * value2.Size);
        }
    }

    public class RectAnimation : TransitionAnimationTimeline<Rect>
    {
        public RectAnimation() :
            base(RectAnimationOperations.Default, true)
        {
            //
        }
    }

    public class RectAnimationUsingKeyFrames : KeyFramesAnimationTimeline<Rect>
    {
        public RectAnimationUsingKeyFrames() :
            base(RectAnimationOperations.Default, true)
        {
            //
        }
    }

    public class DiscreteRectKeyFrame : DiscreteKeyFrame<Rect>
    {
        public DiscreteRectKeyFrame() :
            base(RectAnimationOperations.Default)
        {
            //
        }
    }

    public class LinearRectKeyFrame : LinearKeyFrame<Rect>
    {
        public LinearRectKeyFrame() :
            base(RectAnimationOperations.Default)
        {
            //
        }
    }

    public class EasingRectKeyFrame : EasingKeyFrame<Rect>
    {
        public EasingRectKeyFrame() :
            base(RectAnimationOperations.Default)
        {
            //
        }
    }
}
