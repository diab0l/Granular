using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Shapes
{
    public class Ellipse : Shape
    {
        private Geometry definingGeometry;
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (definingGeometry == null)
                {
                    definingGeometry = new EllipseGeometry(new Point(RenderSize.Width / 2, RenderSize.Height / 2), RenderSize.Width / 2 - StrokeThickness / 2, RenderSize.Height / 2 - StrokeThickness / 2);
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
