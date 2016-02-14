using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Media
{
    public class TranslateTransform : Transform
    {
        private Matrix matrix;
        public override Matrix Value { get { return matrix; } }

        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(TranslateTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(TranslateTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public TranslateTransform() :
            this(Matrix.Identity)
        {
            //
        }

        public TranslateTransform(double x, double y) :
            this(Matrix.TranslationMatrix(x, y))
        {
            //
        }

        private TranslateTransform(Matrix matrix)
        {
            this.matrix = matrix;
        }

        private void SetMatrix()
        {
            matrix = Matrix.TranslationMatrix(X, Y);
        }

        private static void SetMatrix(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((TranslateTransform)dependencyObject).SetMatrix();
        }
    }
}
