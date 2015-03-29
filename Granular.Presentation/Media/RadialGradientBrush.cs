using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class RadialGradientBrush : GradientBrush
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(new Point(0.5, 0.5)));
        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty GradientOriginProperty = DependencyProperty.Register("GradientOrigin", typeof(Point), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(new Point(0.5, 0.5)));
        public Point GradientOrigin
        {
            get { return (Point)GetValue(GradientOriginProperty); }
            set { SetValue(GradientOriginProperty, value); }
        }

        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(0.5));
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(0.5));
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        public RadialGradientBrush()
        {
            //
        }

        public RadialGradientBrush(Color startColor, Color endColor) :
            this(new GradientStop[] { new GradientStop(startColor, 0), new GradientStop(endColor, 1) })
        {
            //
        }

        public RadialGradientBrush(IEnumerable<GradientStop> gradientStops) :
            base(gradientStops)
        {
            //
        }
    }
}
