using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class RotateTransform : Transform
    {
        private Matrix matrix;
        public override Matrix Value { get { return matrix; } }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(RotateTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(double), typeof(RotateTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(double), typeof(RotateTransform), new FrameworkPropertyMetadata(0.0, SetMatrix));
        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        public RotateTransform()
        {
            matrix = Matrix.Identity;
        }

        private void SetMatrix()
        {
            matrix = Matrix.RotationMatrix(Math.PI * Angle / 180, CenterX, CenterY);
            InvalidateRenderResource();
        }

        private static void SetMatrix(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as RotateTransform).SetMatrix();
        }
    }
}
