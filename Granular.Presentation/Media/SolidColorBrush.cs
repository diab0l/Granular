using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class SolidColorBrush : Brush
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(SolidColorBrush), new FrameworkPropertyMetadata(Colors.Transparent, (sender, e) => ((SolidColorBrush)sender).OnColorChanged(e)));
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        private ISolidColorBrushRenderResource renderResource;

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

        protected override object CreateRenderResource(IRenderElementFactory factory)
        {
            return factory.CreateSolidColorBrushRenderResource();
        }

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (ISolidColorBrushRenderResource)renderResource;
            this.renderResource.Color = Color;
        }

        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.Color = Color;
            }
        }
    }
}
