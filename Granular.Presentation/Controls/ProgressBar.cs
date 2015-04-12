using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Granular.Extensions;
using System.Windows.Media;

namespace System.Windows.Controls
{
    [TemplatePart("PART_Track", typeof(FrameworkElement))]
    [TemplatePart("PART_Indicator", typeof(FrameworkElement))]
    [TemplatePart("PART_Glow", typeof(Border))]
    public class ProgressBar : RangeBase
    {
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProgressBar), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((ProgressBar)sender).OnIsIndeterminateChanged(e)));
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        private FrameworkElement track;
        private FrameworkElement Track
        {
            get { return track; }
            set
            {
                if (track == value)
                {
                    return;
                }

                if (track != null)
                {
                    track.LayoutUpdated -= OnTrackLayoutUpdated;
                }

                track = value;

                if (track != null)
                {
                    track.LayoutUpdated += OnTrackLayoutUpdated;
                }
            }
        }

        private FrameworkElement indicator;
        private FrameworkElement Indicator
        {
            get { return indicator; }
            set
            {
                if (indicator == value)
                {
                    return;
                }

                if (indicator != null)
                {
                    indicator.LayoutUpdated -= OnIndicatorLayoutUpdated;
                }

                indicator = value;

                if (indicator != null)
                {
                    indicator.LayoutUpdated += OnIndicatorLayoutUpdated;
                }
            }
        }


        private Border glow;
        private double currentAnimatedIndicatorWidth;
        private AnimationTimelineClock currentAnimationClock;

        static ProgressBar()
        {
            FrameworkElement.FocusableProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(false));
            RangeBase.MaximumProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(100.0));
            Control.ForegroundProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((ProgressBar)sender).OnForegroundChanged(e)));
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                Track = Template.FindName("PART_Track", this) as FrameworkElement;
                Indicator = Template.FindName("PART_Indicator", this) as FrameworkElement;
                glow = Template.FindName("PART_Glow", this) as Border;
            }
            else
            {
                Track = null;
                Indicator = null;
                glow = null;
            }

            SetIndicatorSize();
            SetGlowColor();
            SetGlowAnimation();
        }

        protected override void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            SetIndicatorSize();
        }

        protected override void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            SetIndicatorSize();
        }

        protected override void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            SetIndicatorSize();
        }

        private void OnTrackLayoutUpdated(object sender, EventArgs e)
        {
            SetIndicatorSize();
        }

        private void OnIndicatorLayoutUpdated(object sender, EventArgs e)
        {
            SetGlowAnimation();
        }

        private void OnIsIndeterminateChanged(DependencyPropertyChangedEventArgs e)
        {
            SetIndicatorSize();
            SetGlowColor();
        }

        private void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            SetGlowColor();
        }

        private void SetIndicatorSize()
        {
            if (Track == null || Indicator == null)
            {
                return;
            }

            double width = IsIndeterminate ? Track.ActualWidth : Track.ActualWidth * (Value - Minimum) / (Maximum - Minimum);

            if (!Indicator.Width.IsClose(width))
            {
                Indicator.Width = width;
            }
        }

        private void SetGlowColor()
        {
            if (glow == null)
            {
                return;
            }

            if (IsIndeterminate && !(Foreground is SolidColorBrush))
            {
                glow.Background = Foreground;
            }
            else
            {
                Color color = IsIndeterminate ? ((SolidColorBrush)Foreground).Color : Color.FromArgb(128, 255, 255, 255);

                glow.Background = new LinearGradientBrush(Point.Zero, new Point(1, 0), new[]
                {
                    new GradientStop(Colors.Transparent, 0),
                    new GradientStop(color, 0.4),
                    new GradientStop(color, 0.6),
                    new GradientStop(Colors.Transparent, 1),
                });
            }
        }

        private void SetGlowAnimation()
        {
            if (Indicator == null || glow == null || Indicator.ActualWidth.IsClose(currentAnimatedIndicatorWidth))
            {
                return;
            }

            currentAnimatedIndicatorWidth = Indicator.ActualWidth;

            double currentOffset = glow.Margin.Left;

            if (currentAnimationClock != null)
            {
                ((IAnimatable)glow).RootClock.RemoveClock(currentAnimationClock);
                currentAnimationClock = null;
            }

            if (Indicator.ActualWidth > 0)
            {
                double startOffset = -glow.ActualWidth;
                double endOffset = Indicator.ActualWidth;

                TimeSpan time = TimeSpan.FromSeconds((endOffset - startOffset) / 200);

                ThicknessAnimationUsingKeyFrames thicknessAnimation = new ThicknessAnimationUsingKeyFrames();

                thicknessAnimation.KeyFrames.Add(new LinearThicknessKeyFrame { Value = new Thickness(startOffset, 0, 0, 0), KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
                thicknessAnimation.KeyFrames.Add(new LinearThicknessKeyFrame { Value = new Thickness(endOffset, 0, 0, 0), KeyTime = KeyTime.FromTimeSpan(time) });

                thicknessAnimation.Duration = Duration.FromTimeSpan(time + TimeSpan.FromSeconds(1));

                thicknessAnimation.RepeatBehavior = RepeatBehavior.Forever;
                thicknessAnimation.BeginTime = -time.Scale((currentOffset - startOffset) / (endOffset - startOffset));

                currentAnimationClock = (AnimationTimelineClock)thicknessAnimation.CreateClock();

                glow.ApplyAnimationClock(FrameworkElement.MarginProperty, currentAnimationClock);
                currentAnimationClock.Begin(((IAnimatable)glow).RootClock);
            }
            else
            {
                glow.ClearAnimationClocks(FrameworkElement.MarginProperty);
            }
        }
    }
}
