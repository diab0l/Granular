using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xaml;

namespace System.Windows.Media
{
    [TypeConverter(typeof(BrushTypeConverter))]
    public abstract class Brush : Animatable
    {
        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(Brush), new FrameworkPropertyMetadata(1.0));
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }
    }

    public class BrushTypeConverter : ITypeConverter
    {
        private ColorTypeConverter colorTypeConverter;

        public BrushTypeConverter()
        {
            colorTypeConverter = new ColorTypeConverter();
        }

        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return new SolidColorBrush((Color)colorTypeConverter.ConvertFrom(namespaces, value));
        }
    }
}
