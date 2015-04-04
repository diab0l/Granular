using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlRenderElementFactory : IRenderElementFactory
    {
        public static readonly HtmlRenderElementFactory Default = new HtmlRenderElementFactory();

        private HtmlRenderElementFactory()
        {
            //
        }

        public IVisualRenderElement CreateVisualRenderElement(object owner)
        {
            return new HtmlVisualRenderElement(owner, RenderQueue.Default, HtmlValueConverter.Default);
        }

        public IDrawingRenderElement CreateDrawingRenderElement(object owner)
        {
            throw new NotImplementedException();
        }

        public ITextBoxRenderElement CreateTextBoxRenderElement(object owner)
        {
            return new HtmlTextBoxRenderElement(RenderQueue.Default, HtmlValueConverter.Default);
        }

        public ITextBlockRenderElement CreateTextBlockRenderElement(object owner)
        {
            return new HtmlTextBlockRenderElement(RenderQueue.Default, HtmlValueConverter.Default);
        }

        public IBorderRenderElement CreateBorderRenderElement(object owner)
        {
            return new HtmlBorderRenderElement(RenderQueue.Default, HtmlValueConverter.Default);
        }

        public IImageRenderElement CreateImageRenderElement(object owner)
        {
            return new HtmlImageRenderElement(RenderQueue.Default, HtmlValueConverter.Default);
        }
    }
}
