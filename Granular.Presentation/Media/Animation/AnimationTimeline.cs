using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public abstract class AnimationTimeline : Timeline
    {
        public static readonly DependencyProperty IsAdditiveProperty = DependencyProperty.Register("IsAdditive", typeof(bool), typeof(AnimationTimeline), new FrameworkPropertyMetadata());
        public bool IsAdditive
        {
            get { return (bool)GetValue(IsAdditiveProperty); }
            set { SetValue(IsAdditiveProperty, value); }
        }

        public static readonly DependencyProperty IsCumulativeProperty = DependencyProperty.Register("IsCumulative", typeof(bool), typeof(AnimationTimeline), new FrameworkPropertyMetadata());
        public bool IsCumulative
        {
            get { return (bool)GetValue(IsCumulativeProperty); }
            set { SetValue(IsCumulativeProperty, value); }
        }

        public abstract object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock);

        public override TimelineClock CreateClock()
        {
            return new AnimationTimelineClock(this);
        }
    }
}
