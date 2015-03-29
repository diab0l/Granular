using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace System.Windows.Controls.Primitives
{
    public class RepeatButton : ButtonBase
    {
        // interval between the first click and the second click
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(RepeatButton), new FrameworkPropertyMetadata(500));
        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(int), typeof(RepeatButton), new FrameworkPropertyMetadata(33));
        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        private DispatcherTimer timer;

        static RepeatButton()
        {
            ClickModeProperty.OverrideMetadata(typeof(RepeatButton), new FrameworkPropertyMetadata(ClickMode.Press));
        }

        public RepeatButton()
        {
            timer = new DispatcherTimer();
            timer.Tick += OnTimerTick;
        }

        protected override void OnPressStarted()
        {
            timer.Interval = TimeSpan.FromMilliseconds(Delay > 0 ? Delay : Interval);
            timer.Start();
        }

        protected override void OnPressEnded()
        {
            timer.Stop();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // the first interval can be different
            if (timer.Interval.TotalMilliseconds != Interval)
            {
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(Interval);
                timer.Start();
            }

            if (IsPressed)
            {
                RaiseClick();
            }
        }
    }
}
