using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Media
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class LinearGradientBrush : GradientBrush
    {
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register("StartPoint", typeof(Point), typeof(LinearGradientBrush), new FrameworkPropertyMetadata(Point.Zero, (sender, e) => ((LinearGradientBrush)sender).OnStartPointChanged(e)));
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(Point), typeof(LinearGradientBrush), new FrameworkPropertyMetadata(new Point(1, 1), (sender, e) => ((LinearGradientBrush)sender).OnEndPointChanged(e)));
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        private ILinearGradientBrushRenderResource renderResource;

        public LinearGradientBrush()
        {
            //
        }

        public LinearGradientBrush(double angle, Color startColor, Color endColor) :
            this(GetStartPoint(angle), GetEndPoint(angle), new [] { new GradientStop(startColor, 0), new GradientStop(endColor, 1) })
        {
            //
        }

        public LinearGradientBrush(double angle, IEnumerable<GradientStop> gradientStops) :
            this(GetStartPoint(angle), GetEndPoint(angle), gradientStops)
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

        private static Point GetStartPoint(double angle)
        {
            Point startPoint;
            Point endPoint;

            GetPoints(angle, out startPoint, out endPoint);

            return startPoint;
        }

        private static Point GetEndPoint(double angle)
        {
            Point startPoint;
            Point endPoint;

            GetPoints(angle, out startPoint, out endPoint);

            return endPoint;
        }

        private static void GetPoints(double angle, out Point startPoint, out Point endPoint)
        {
            double radians = Math.PI * angle / 180;
            double x = Math.Cos(radians);
            double y = Math.Sin(radians);

            double scale = 1 / x.Abs().Max(y.Abs());
            x *= scale;
            y *= scale;

            Point offset = new Point(x.Min(0), y.Min(0));

            startPoint = -offset;
            endPoint = new Point(x, y) - offset;
        }

        protected override object CreateRenderResource(IRenderElementFactory factory)
        {
            return factory.CreateLinearGradientBrushRenderResource();
        }

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (ILinearGradientBrushRenderResource)renderResource;
            this.renderResource.StartPoint = StartPoint;
            this.renderResource.EndPoint = EndPoint;
        }

        private void OnStartPointChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.StartPoint = StartPoint;
            }
        }

        private void OnEndPointChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.EndPoint = EndPoint;
            }
        }
    }
}
