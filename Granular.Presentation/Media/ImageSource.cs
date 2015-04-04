using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Xaml;

namespace System.Windows.Media
{
    public abstract class ImageSource : Animatable
    {
        public abstract IRenderImageSource RenderImageSource { get; }

        public Size Size { get { return RenderImageSource != null ? RenderImageSource.Size : Size.Empty; } }
    }
}
