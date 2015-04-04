using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Xaml;
using System.Windows.Media.Imaging;

namespace System.Windows.Media
{
    [TypeConverter(typeof(ImageSourceTypeConverter))]
    public abstract class ImageSource : Animatable
    {
        public abstract IRenderImageSource RenderImageSource { get; }

        public Size Size { get { return RenderImageSource != null ? RenderImageSource.Size : Size.Empty; } }
    }

    public class ImageSourceTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return new BitmapImage { UriSource = (string)value };
        }
    }
}
