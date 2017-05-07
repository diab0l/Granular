using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Media
{
    public class EllipseGeometry : Geometry
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(EllipseGeometry), new FrameworkPropertyMetadata((sender, e) => ((EllipseGeometry)sender).InvalidateRenderResource()));
        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(EllipseGeometry), new FrameworkPropertyMetadata((sender, e) => ((EllipseGeometry)sender).InvalidateRenderResource()));
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(EllipseGeometry), new FrameworkPropertyMetadata((sender, e) => ((EllipseGeometry)sender).InvalidateRenderResource()));
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        public EllipseGeometry()
        {
            //
        }

        public EllipseGeometry(Point center, double radiusX, double radiusY) :
            this()
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public EllipseGeometry(Point center, double radiusX, double radiusY, Transform transform) :
            this(center, radiusX, radiusY)
        {
            Transform = transform;
        }

        public EllipseGeometry(Rect rect) :
            this(new Point((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2), rect.Width / 2, rect.Height / 2)
        {
            //
        }

        protected override string GetRenderResourceData()
        {
            double rx = RadiusX;
            double ry = RadiusY;

            return $"M {Center.X - rx},{Center.Y} a {rx},{ry} 0 0,1 {2 * rx},0 a {rx},{ry} 0 0,1 {-2 * rx},0 z";
        }
    }
}
