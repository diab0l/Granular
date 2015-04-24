using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    public interface IEasingFunction
    {
        double Ease(double normalizedTime);
    }

    public enum EasingMode
    {
        EaseIn,
        EaseOut,
        EaseInOut
    }

    public abstract class EasingFunctionBase : Freezable, IEasingFunction
    {
        public static readonly DependencyProperty EasingModeProperty = DependencyProperty.Register("EasingMode", typeof(EasingMode), typeof(EasingFunctionBase), new FrameworkPropertyMetadata(EasingMode.EaseOut));
        public EasingMode EasingMode
        {
            get { return (EasingMode)GetValue(EasingModeProperty); }
            set { SetValue(EasingModeProperty, value); }
        }

        public double Ease(double normalizedTime)
        {
            switch (EasingMode)
            {
                case EasingMode.EaseIn: return EaseOverride(normalizedTime);
                case EasingMode.EaseOut: return 1 - EaseOverride(1 - normalizedTime);
                case EasingMode.EaseInOut: return normalizedTime < 0.5 ? EaseOverride(normalizedTime * 2) / 2 : (2 - EaseOverride((1 - normalizedTime) * 2)) / 2;
            }

            throw new Granular.Exception("Unexpected EasingMode \"{0}\"", EasingMode);
        }

        protected abstract double EaseOverride(double normalizedTime);
    }

    public class BackEase : EasingFunctionBase
    {
        public static readonly DependencyProperty AmplitudeProperty = DependencyProperty.Register("Amplitude", typeof(double), typeof(BackEase), new FrameworkPropertyMetadata(1.0));
        public double Amplitude
        {
            get { return (double)GetValue(AmplitudeProperty); }
            set { SetValue(AmplitudeProperty, value); }
        }

        protected override double EaseOverride(double normalizedTime)
        {
            return Math.Pow(normalizedTime, 3.0) - normalizedTime * Amplitude.Max(0) * Math.Sin(Math.PI * normalizedTime);
        }
    }

    public class CircleEase : EasingFunctionBase
    {
        protected override double EaseOverride(double normalizedTime)
        {
            normalizedTime = normalizedTime.Bounds(0, 1);
            return 1.0 - Math.Sqrt(1.0 - normalizedTime * normalizedTime);
        }
    }

    public class ElasticEase : EasingFunctionBase
    {
        public static readonly DependencyProperty OscillationsProperty = DependencyProperty.Register("Oscillations", typeof(double), typeof(ElasticEase), new FrameworkPropertyMetadata(3.0));
        public double Oscillations
        {
            get { return (double)GetValue(OscillationsProperty); }
            set { SetValue(OscillationsProperty, value); }
        }

        public static readonly DependencyProperty SpringinessProperty = DependencyProperty.Register("Springiness", typeof(double), typeof(ElasticEase), new FrameworkPropertyMetadata(3.0));
        public double Springiness
        {
            get { return (double)GetValue(SpringinessProperty); }
            set { SetValue(SpringinessProperty, value); }
        }

        protected override double EaseOverride(double normalizedTime)
        {
            double springiness = Springiness.Max(0);
            return (springiness.IsClose(0) ? normalizedTime : (Math.Exp(springiness * normalizedTime) - 1.0) / (Math.Exp(springiness) - 1.0)) * (Math.Sin((Math.PI * 2.0 * Oscillations.Max(0) + Math.PI * 0.5) * normalizedTime));
        }
    }

    public class ExponentialEase : EasingFunctionBase
    {
        public static readonly DependencyProperty ExponentProperty = DependencyProperty.Register("Exponent", typeof(double), typeof(ExponentialEase), new FrameworkPropertyMetadata(2.0));
        public double Exponent
        {
            get { return (double)GetValue(ExponentProperty); }
            set { SetValue(ExponentProperty, value); }
        }

        protected override double EaseOverride(double normalizedTime)
        {
            return Exponent.IsClose(0) ? normalizedTime : (Math.Exp(Exponent * normalizedTime) - 1.0) / (Math.Exp(Exponent) - 1.0);
        }
    }

    public class PowerEase : EasingFunctionBase
    {
        public static readonly DependencyProperty PowerProperty = DependencyProperty.Register("Power", typeof(double), typeof(PowerEase), new FrameworkPropertyMetadata(2.0));
        public double Power
        {
            get { return (double)GetValue(PowerProperty); }
            set { SetValue(PowerProperty, value); }
        }

        protected override double EaseOverride(double normalizedTime)
        {
            return Math.Pow(normalizedTime, Power.Max(0));
        }
    }

    public class SineEase : EasingFunctionBase
    {
        protected override double EaseOverride(double normalizedTime)
        {
            return 1.0 - Math.Sin(Math.PI * 0.5 * (1 - normalizedTime));
        }
    }

    public class QuadraticEase : EasingFunctionBase
    {
        protected override double EaseOverride(double normalizedTime)
        {
            return normalizedTime * normalizedTime;
        }
    }

    public class CubicEase : EasingFunctionBase
    {
        protected override double EaseOverride(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime;
        }
    }

    public class QuarticEase : EasingFunctionBase
    {
        protected override double EaseOverride(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }
    }

    public class QuinticEase : EasingFunctionBase
    {
        protected override double EaseOverride(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }
    }
}
