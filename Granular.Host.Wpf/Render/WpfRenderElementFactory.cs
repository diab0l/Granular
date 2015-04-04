using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfRenderElementFactory : IRenderElementFactory
    {
        public static readonly WpfRenderElementFactory Default = new WpfRenderElementFactory();

        private WpfRenderElementFactory()
        {
            //
        }

        public IVisualRenderElement CreateVisualRenderElement(object owner)
        {
            return new WpfVisualRenderElement(owner, WpfValueConverter.Default);
        }

        public IDrawingRenderElement CreateDrawingRenderElement(object owner)
        {
            return null;
        }

        public ITextBoxRenderElement CreateTextBoxRenderElement(object owner)
        {
            return new WpfTextBoxRenderElement(WpfValueConverter.Default);
        }

        public ITextBlockRenderElement CreateTextBlockRenderElement(object owner)
        {
            return new WpfTextBlockRenderElement(WpfValueConverter.Default);
        }

        public IBorderRenderElement CreateBorderRenderElement(object owner)
        {
            return new WpfBorderRenderElement(WpfValueConverter.Default);
        }

        public IImageRenderElement CreateImageRenderElement(object owner)
        {
            return new WpfImageRenderElement(WpfValueConverter.Default);
        }
    }
}
