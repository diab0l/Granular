using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Media
{
    public class LineGeometry : Geometry
    {
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register("StartPoint", typeof(Point), typeof(LineGeometry), new FrameworkPropertyMetadata((sender, e) => ((LineGeometry)sender).InvalidateRenderResource()));
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(Point), typeof(LineGeometry), new FrameworkPropertyMetadata((sender, e) => ((LineGeometry)sender).InvalidateRenderResource()));
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        public LineGeometry()
        {
            //
        }

        public LineGeometry(Point startPoint, Point endPoint) :
            this()
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        public LineGeometry(Point startPoint, Point endPoint, Transform transform) :
            this(startPoint, endPoint)
        {
            this.Transform = transform;
        }

        protected override string GetRenderResourceData()
        {
            return $"M {StartPoint.X}, {StartPoint.Y} {EndPoint.X}, {EndPoint.Y}";
        }
    }
}
