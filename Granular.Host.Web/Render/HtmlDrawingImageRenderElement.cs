using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Render
{
    internal class HtmlDrawingImageRenderElement : HtmlRenderElement, IDrawingImageRenderElement
    {
        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                if (imageSource == value)
                {
                    return;
                }

                imageSource = value;
                renderQueue.InvokeAsync(() => HtmlElement.SetSvgImageSource(imageSource, factory, converter));
            }
        }

        private Rect rectangle;
        public Rect Rectangle
        {
            get { return rectangle; }
            set
            {
                if (rectangle == value)
                {
                    return;
                }

                rectangle = value;
                renderQueue.InvokeAsync(() => HtmlElement.SetSvgBounds(rectangle, converter));
            }
        }

        private IRenderElementFactory factory;
        private RenderQueue renderQueue;
        private SvgValueConverter converter;

        public HtmlDrawingImageRenderElement(IRenderElementFactory htmlRenderElementFactory, RenderQueue renderQueue, SvgValueConverter svgValueConverter) :
            base(SvgDocument.CreateElement("image"))
        {
            this.factory = htmlRenderElementFactory;
            this.renderQueue = renderQueue;
            this.converter = svgValueConverter;
        }
    }
}