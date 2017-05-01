using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class RadialGradientBrush : GradientBrush
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(new Point(0.5, 0.5), (sender, e) => ((RadialGradientBrush)sender).OnCenterChanged(e)));
        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty GradientOriginProperty = DependencyProperty.Register("GradientOrigin", typeof(Point), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(new Point(0.5, 0.5), (sender, e) => ((RadialGradientBrush)sender).OnGradientOriginChanged(e)));
        public Point GradientOrigin
        {
            get { return (Point)GetValue(GradientOriginProperty); }
            set { SetValue(GradientOriginProperty, value); }
        }

        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(0.5, (sender, e) => ((RadialGradientBrush)sender).OnRadiusXChanged(e)));
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(RadialGradientBrush), new FrameworkPropertyMetadata(0.5, (sender, e) => ((RadialGradientBrush)sender).OnRadiusYChanged(e)));
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        private IRadialGradientBrushRenderResource renderResource;

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

        protected override object CreateRenderResource(IRenderElementFactory factory)
        {
            return factory.CreateRadialGradientBrushRenderResource();
        }

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (IRadialGradientBrushRenderResource)renderResource;
            this.renderResource.Center = Center;
            this.renderResource.GradientOrigin = GradientOrigin;
            this.renderResource.RadiusX = RadiusX;
            this.renderResource.RadiusY = RadiusY;
        }

        private void OnCenterChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.Center = Center;
            }
        }

        private void OnGradientOriginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.GradientOrigin = GradientOrigin;
            }
        }

        private void OnRadiusXChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.RadiusX = RadiusX;
            }
        }

        private void OnRadiusYChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.RadiusY = RadiusY;
            }
        }
    }
}
