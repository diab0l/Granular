using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class SkewTransform : Transform
    {
        private Matrix matrix;
        public override Matrix Value { get { return matrix; } }

        public static readonly DependencyProperty AngleXProperty = DependencyProperty.Register("AngleX", typeof(double), typeof(SkewTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double AngleX
        {
            get { return (double)GetValue(AngleXProperty); }
            set { SetValue(AngleXProperty, value); }
        }

        public static readonly DependencyProperty AngleYProperty = DependencyProperty.Register("AngleY", typeof(double), typeof(SkewTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double AngleY
        {
            get { return (double)GetValue(AngleYProperty); }
            set { SetValue(AngleYProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(double), typeof(SkewTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(double), typeof(SkewTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        public SkewTransform()
        {
            matrix = Matrix.Identity;
        }

        private void SetMatrix()
        {
            matrix = Matrix.SkewMatrix(Math.PI * AngleX / 180, Math.PI * AngleY / 180, CenterX, CenterY);
            InvalidateRenderResource();
        }

        private static void SetMatrix(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as SkewTransform).SetMatrix();
        }
    }
}
