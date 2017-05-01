extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfSolidColorBrushRenderResource : WpfBrushRenderResource, ISolidColorBrushRenderResource
    {
        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                if (color == value)
                {
                    return;
                }

                color = value;
                brush.Color = converter.Convert(color);
            }
        }

        private wpf::System.Windows.Media.SolidColorBrush brush;
        private WpfValueConverter converter;

        public WpfSolidColorBrushRenderResource(WpfValueConverter converter) :
            this(new wpf::System.Windows.Media.SolidColorBrush(), converter)
        {
            //
        }

        public WpfSolidColorBrushRenderResource(wpf::System.Windows.Media.SolidColorBrush brush, WpfValueConverter converter) :
            base(brush)
        {
            this.brush = brush;
            this.converter = converter;
        }
    }
}
