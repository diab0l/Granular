using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class SolidColorBrush : Brush
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(SolidColorBrush), new FrameworkPropertyMetadata(Colors.Transparent));
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public SolidColorBrush()
        {
            //
        }

        public SolidColorBrush(Color color)
        {
            this.Color = color;
        }

        public override string ToString()
        {
            return String.Format("SolidColorBrush({0})", Color);
        }
    }
}
