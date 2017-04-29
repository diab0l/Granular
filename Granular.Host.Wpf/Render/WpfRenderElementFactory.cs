using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfRenderElementFactory : IRenderElementFactory
    {
        private WpfValueConverter wpfValueConverter;

        public WpfRenderElementFactory(WpfValueConverter wpfValueConverter)
        {
            this.wpfValueConverter = wpfValueConverter;
        }

        public IVisualRenderElement CreateVisualRenderElement(object owner)
        {
            return new WpfVisualRenderElement(owner, wpfValueConverter);
        }

        public IDrawingRenderElement CreateDrawingRenderElement(object owner)
        {
            return null;
        }

        public ITextBoxRenderElement CreateTextBoxRenderElement(object owner)
        {
            return new WpfTextBoxRenderElement(wpfValueConverter);
        }

        public ITextBlockRenderElement CreateTextBlockRenderElement(object owner)
        {
            return new WpfTextBlockRenderElement(wpfValueConverter);
        }

        public IBorderRenderElement CreateBorderRenderElement(object owner)
        {
            return new WpfBorderRenderElement(wpfValueConverter);
        }

        public IImageRenderElement CreateImageRenderElement(object owner)
        {
            return new WpfImageRenderElement(wpfValueConverter);
        }
    }
}
