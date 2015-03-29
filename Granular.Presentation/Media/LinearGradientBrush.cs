using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class LinearGradientBrush : GradientBrush
    {
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register("StartPoint", typeof(Point), typeof(LinearGradientBrush), new FrameworkPropertyMetadata(Point.Zero, (sender, e) => ((LinearGradientBrush)sender).SetAngle()));
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(Point), typeof(LinearGradientBrush), new FrameworkPropertyMetadata(new Point(1, 1), (sender, e) => ((LinearGradientBrush)sender).SetAngle()));
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(LinearGradientBrush), new FrameworkPropertyMetadata(45.0));
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public LinearGradientBrush()
        {
            //
        }

        public LinearGradientBrush(double angle, Color startColor, Color endColor) :
            this(Point.Zero, GetEndPoint(angle), new [] { new GradientStop(startColor, 0), new GradientStop(endColor, 1) })
        {
            //
        }

        public LinearGradientBrush(double angle, IEnumerable<GradientStop> gradientStops) :
            this(Point.Zero, GetEndPoint(angle), gradientStops)
        {
            //
        }

        public LinearGradientBrush(Point startPoint, Point endPoint, Color startColor, Color endColor) :
            this(startPoint, endPoint, new[] { new GradientStop(startColor, 0), new GradientStop(endColor, 1) })
        {
            //
        }

        public LinearGradientBrush(Point startPoint, Point endPoint, IEnumerable<GradientStop> gradientStops) :
            base(gradientStops)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        private void SetAngle()
        {
            if (StartPoint.IsClose(EndPoint))
            {
                Angle = 0;
            }
            else
            {
                Point offset = EndPoint - StartPoint;
                Angle = Math.Sign(offset.Y) * Math.Acos(offset.X / Math.Sqrt(offset.X * offset.X + offset.Y * offset.Y)) * 180.0 / Math.PI;
            }
        }

        private static Point GetEndPoint(double angle)
        {
            double radians = Math.PI * angle / 180;
            return new Point(Math.Cos(radians), Math.Sin(radians));
        }
    }
}
