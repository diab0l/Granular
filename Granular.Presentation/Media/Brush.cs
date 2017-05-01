using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace System.Windows.Media
{
    [TypeConverter(typeof(BrushTypeConverter))]
    public abstract class Brush : Animatable
    {
        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(Brush), new FrameworkPropertyMetadata(1.0, (sender, e) => ((Brush)sender).OnOpacityChanged(e)));
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        private IBrushRenderResource renderResource;

        public object GetRenderResource(IRenderElementFactory factory)
        {
            if (renderResource == null)
            {
                object resource = CreateRenderResource(factory);
                OnRenderResourceCreated(resource);

                if (renderResource == null)
                {
                    throw new Granular.Exception("base.OnRenderResourceCreated was not called for \"{0}\"", GetType().Name);
                }
            }

            return renderResource;
        }

        protected abstract object CreateRenderResource(IRenderElementFactory factory);

        protected virtual void OnRenderResourceCreated(object renderResource)
        {
            this.renderResource = (IBrushRenderResource)renderResource;
            this.renderResource.Opacity = Opacity;
        }

        private void OnOpacityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.Opacity = Opacity;
            }
        }
    }

    public class BrushTypeConverter : ITypeConverter
    {
        private ColorTypeConverter colorTypeConverter;

        public BrushTypeConverter()
        {
            colorTypeConverter = new ColorTypeConverter();
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            if (value is string)
            {
                return new SolidColorBrush((Color)colorTypeConverter.ConvertFrom(namespaces, sourceUri, value));
            }

            throw new Granular.Exception("Can't convert \"{0}\" to Color", value);
        }
    }
}
