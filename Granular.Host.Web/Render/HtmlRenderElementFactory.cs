using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlRenderElementFactory : IRenderElementFactory
    {
        private RenderQueue renderQueue;
        private HtmlValueConverter htmlValueConverter;

        public HtmlRenderElementFactory(RenderQueue renderQueue, HtmlValueConverter htmlValueConverter)
        {
            this.renderQueue = renderQueue;
            this.htmlValueConverter = htmlValueConverter;
        }

        public IVisualRenderElement CreateVisualRenderElement(object owner)
        {
            return new HtmlVisualRenderElement(owner, renderQueue, htmlValueConverter);
        }

        public IDrawingRenderElement CreateDrawingRenderElement(object owner)
        {
            throw new NotImplementedException();
        }

        public ITextBoxRenderElement CreateTextBoxRenderElement(object owner)
        {
            return new HtmlTextBoxRenderElement(renderQueue, htmlValueConverter);
        }

        public ITextBlockRenderElement CreateTextBlockRenderElement(object owner)
        {
            return new HtmlTextBlockRenderElement(renderQueue, htmlValueConverter);
        }

        public IBorderRenderElement CreateBorderRenderElement(object owner)
        {
            return new HtmlBorderRenderElement(renderQueue, htmlValueConverter);
        }

        public IImageRenderElement CreateImageRenderElement(object owner)
        {
            return new HtmlImageRenderElement(renderQueue, htmlValueConverter);
        }
    }
}
