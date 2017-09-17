using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Shapes
{
    public class Line : Shape
    {
        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0.0, (sender, e) => ((Line)sender).InvalidateDefiningGeometry()));
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0.0, (sender, e) => ((Line)sender).InvalidateDefiningGeometry()));
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0.0, (sender, e) => ((Line)sender).InvalidateDefiningGeometry()));
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0.0, (sender, e) => ((Line)sender).InvalidateDefiningGeometry()));
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        private Geometry definingGeometry;
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (definingGeometry == null)
                {
                    definingGeometry = new LineGeometry(new Point(X1, Y1), new Point(X2, Y2));
                }

                return definingGeometry;
            }
        }

        private void InvalidateDefiningGeometry()
        {
            definingGeometry = null;
            InvalidateVisual();
        }
    }
}
