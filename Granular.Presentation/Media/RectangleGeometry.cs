using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Media
{
    public class RectangleGeometry : Geometry
    {
        public static readonly DependencyProperty RectProperty = DependencyProperty.Register("Rect", typeof(Rect), typeof(RectangleGeometry), new FrameworkPropertyMetadata((sender, e) => ((RectangleGeometry)sender).InvalidateRenderResource()));
        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(RectangleGeometry), new FrameworkPropertyMetadata((sender, e) => ((RectangleGeometry)sender).InvalidateRenderResource()));
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(RectangleGeometry), new FrameworkPropertyMetadata((sender, e) => ((RectangleGeometry)sender).InvalidateRenderResource()));
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        public RectangleGeometry()
        {
            //
        }

        public RectangleGeometry(Rect rect) :
            this()
        {
            Rect = rect;
        }

        public RectangleGeometry(Rect rect, double radiusX, double radiusY) :
            this(rect)
        {
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public RectangleGeometry(Rect rect, double radiusX, double radiusY, Transform transform) :
            this(rect, radiusX, radiusY)
        {
            Transform = transform;
        }

        protected override string GetRenderResourceData()
        {
            Rect rect = Rect;
            double rx = RadiusX;
            double ry = RadiusY;

            if (rx == 0 && ry == 0)
            {
                return $"M {rect.Left},{rect.Top} l {rect.Width},0 l 0,{rect.Height} l {-rect.Width},0 z";
            }

            return $@"M {rect.Left + rx},{rect.Top}
                      l {rect.Width - 2 * rx},0 a {rx},{ry} 0 0,1 {rx},{ry}
                      l 0,{rect.Height - 2 * ry} a {rx},{ry} 0 0,1 {-rx},{ry}
                      l {-rect.Width + 2 * rx},0 a {rx},{ry} 0 0,1 {-rx},{-ry}
                      l 0,{-rect.Height + 2 * ry} a {rx},{ry} 0 0,1 {rx},{-ry} z";
        }
    }
}
