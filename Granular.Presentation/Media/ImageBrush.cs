using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class ImageBrush : TileBrush
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(ImageBrush), new FrameworkPropertyMetadata());
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
    }
}
