using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media.Animation
{
    public enum FillBehavior
    {
        HoldEnd,
        Stop
    }

    public abstract class Timeline : Freezable
    {
        public static readonly DependencyProperty BeginTimeProperty = DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(Timeline), new FrameworkPropertyMetadata(TimeSpan.Zero));
        public TimeSpan BeginTime
        {
            get { return (TimeSpan)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(Timeline), new FrameworkPropertyMetadata(new Duration(TimeSpan.FromSeconds(1))));
        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty FillBehaviorProperty = DependencyProperty.Register("FillBehavior", typeof(FillBehavior), typeof(Timeline), new FrameworkPropertyMetadata());
        public FillBehavior FillBehavior
        {
            get { return (FillBehavior)GetValue(FillBehaviorProperty); }
            set { SetValue(FillBehaviorProperty, value); }
        }

        public static readonly DependencyProperty RepeatBehaviorProperty = DependencyProperty.Register("RepeatBehavior", typeof(RepeatBehavior), typeof(Timeline), new FrameworkPropertyMetadata(RepeatBehavior.OneTime));
        public RepeatBehavior RepeatBehavior
        {
            get { return (RepeatBehavior)GetValue(RepeatBehaviorProperty); }
            set { SetValue(RepeatBehaviorProperty, value); }
        }

        public static readonly DependencyProperty AutoReverseProperty = DependencyProperty.Register("AutoReverse", typeof(bool), typeof(Timeline), new FrameworkPropertyMetadata());
        public bool AutoReverse
        {
            get { return (bool)GetValue(AutoReverseProperty); }
            set { SetValue(AutoReverseProperty, value); }
        }

        private TimelineGroup parent;
        public TimelineGroup Parent
        {
            get { return parent; }
            set
            {
                if (parent == value)
                {
                    return;
                }

                parent = value;
                TrySetContextParent(value);
            }
        }

        public abstract TimelineClock CreateClock();
    }
}
