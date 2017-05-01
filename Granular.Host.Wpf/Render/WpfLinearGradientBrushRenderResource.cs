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
    public class WpfLinearGradientBrushRenderResource : WpfGradientBrushRenderResource, ILinearGradientBrushRenderResource
    {
        private Point startPoint;
        public Point StartPoint
        {
            get { return startPoint; }
            set
            {
                if (startPoint == value)
                {
                    return;
                }

                startPoint = value;
                brush.StartPoint = converter.Convert(startPoint);
            }
        }

        private Point endPoint;
        public Point EndPoint
        {
            get { return endPoint; }
            set
            {
                if (endPoint == value)
                {
                    return;
                }

                endPoint = value;
                brush.EndPoint = converter.Convert(endPoint);
            }
        }

        private wpf::System.Windows.Media.LinearGradientBrush brush;
        private WpfValueConverter converter;

        public WpfLinearGradientBrushRenderResource(WpfValueConverter converter) :
            this(new wpf::System.Windows.Media.LinearGradientBrush(), converter)
        {
            //
        }

        public WpfLinearGradientBrushRenderResource(wpf::System.Windows.Media.LinearGradientBrush brush, WpfValueConverter converter) :
            base(brush, converter)
        {
            this.brush = brush;
            this.converter = converter;
        }
    }
}
