using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media
{
    public class MatrixTransform : Transform
    {
        public override Matrix Value { get { return Matrix; } }

        public static readonly DependencyProperty MatrixProperty = DependencyProperty.Register("Matrix", typeof(Matrix), typeof(MatrixTransform), new FrameworkPropertyMetadata(Matrix.Identity, (sender, e) => ((MatrixTransform)sender).OnMatrixChanged(e)));
        public Matrix Matrix
        {
            get { return (Matrix)GetValue(MatrixProperty); }
            set { SetValue(MatrixProperty, value); }
        }

        public MatrixTransform() :
            this(Matrix.Identity)
        {
            //
        }

        public MatrixTransform(double m11, double m12, double m21, double m22, double offsetX, double offsetY) :
            this(new Matrix(m11, m12, m21, m22, offsetX, offsetY))
        {
                //
        }

        public MatrixTransform(Matrix matrix)
        {
            this.Matrix = matrix;
        }

        private void OnMatrixChanged(DependencyPropertyChangedEventArgs e)
        {
            InvalidateRenderResource();
        }
    }
}
