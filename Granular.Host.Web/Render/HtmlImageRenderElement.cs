using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlImageRenderElement : HtmlRenderElement, IImageRenderElement
    {
        private Rect bounds;
        public Rect Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds == value)
                {
                    return;
                }

                bounds = value;
                renderQueue.InvokeAsync(() =>
                {
                    HtmlElement.SetHtmlBounds(bounds, converter);
                    SetSourceRect();
                });
            }
        }

        private ImageSource source;
        public ImageSource Source
        {
            get { return source; }
            set
            {
                if (source == value)
                {
                    return;
                }

                source = value;
                renderQueue.InvokeAsync(() =>
                {
                    HtmlElement.SetHtmlBackgroundImage(source, converter);
                    SetSourceRect();
                });
            }
        }

        private RenderQueue renderQueue;
        private HtmlValueConverter converter;

        public HtmlImageRenderElement(RenderQueue renderQueue, HtmlValueConverter converter)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;

            bounds = Rect.Zero;

            HtmlElement.SetHtmlBounds(Bounds, converter);
        }

        private void SetSourceRect()
        {
            if (Source == null || Bounds.IsNullOrEmpty() || Bounds.Size.Width == 0 || Bounds.Size.Height == 0)
            {
                return;
            }

            Rect sourceRect = ((RenderImageSource)Source.RenderImageSource).SourceRect;
            Size imageSize = ((RenderImageSource)Source.RenderImageSource).ImageSize;

            if (!sourceRect.IsNullOrEmpty())
            {
                double widthFactor = Bounds.Size.Width / sourceRect.Width;
                double heightFactor = Bounds.Size.Height / sourceRect.Height;

                Point location = new Point(-sourceRect.Left * widthFactor, -sourceRect.Top * heightFactor);
                Size size = new Size(imageSize.Width * widthFactor, imageSize.Height * heightFactor);

                HtmlElement.SetHtmlBackgroundBounds(new Rect(location, size), converter);
            }
            else
            {
                HtmlElement.SetHtmlBackgroundBounds(new Rect(Bounds.Size), converter);
            }
        }
    }
}
