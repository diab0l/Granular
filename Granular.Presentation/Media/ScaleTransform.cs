using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class ScaleTransform : Transform
    {
        private Matrix matrix;
        public override Matrix Value { get { return matrix; } }

        public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register("ScaleX", typeof(double), typeof(ScaleTransform), new FrameworkPropertyMetadata(1.0, SetMatrix));
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register("ScaleY", typeof(double), typeof(ScaleTransform), new FrameworkPropertyMetadata(1.0, SetMatrix));
        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(double), typeof(ScaleTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(double), typeof(ScaleTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        public ScaleTransform()
        {
            matrix = Matrix.Identity;
        }

        private void SetMatrix()
        {
            matrix = Matrix.ScalingMatrix(ScaleX, ScaleY, CenterX, CenterY);
            InvalidateRenderResource();
        }

        private static void SetMatrix(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as ScaleTransform).SetMatrix();
        }
    }
}
