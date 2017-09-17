using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Shapes
{
    public class Rectangle : Shape
    {
        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(Rectangle), new FrameworkPropertyMetadata(0.0, (sender, e) => ((Rectangle)sender).InvalidateDefiningGeometry()));
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(Rectangle), new FrameworkPropertyMetadata(0.0, (sender, e) => ((Rectangle)sender).InvalidateDefiningGeometry()));
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        private Geometry definingGeometry;
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (definingGeometry == null)
                {
                    definingGeometry = new RectangleGeometry(new Rect(StrokeThickness / 2, StrokeThickness / 2, RenderSize.Width - StrokeThickness, RenderSize.Height - StrokeThickness), RadiusX, RadiusY);
                }

                return definingGeometry;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            InvalidateDefiningGeometry();
            return base.ArrangeOverride(finalSize);
        }

        private void InvalidateDefiningGeometry()
        {
            definingGeometry = null;
            InvalidateVisual();
        }
    }
}
