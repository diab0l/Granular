using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media
{
    public class MatrixTransform : Transform
    {
        public override Matrix Value { get { return Matrix; } }

        public static readonly DependencyProperty MatrixProperty = DependencyProperty.Register("Matrix", typeof(Matrix), typeof(MatrixTransform), new FrameworkPropertyMetadata(Matrix.Identity));
        public Matrix Matrix
        {
            get { return (Matrix)GetValue(MatrixProperty); }
            set { SetValue(MatrixProperty, value); }
        }
    }
}
