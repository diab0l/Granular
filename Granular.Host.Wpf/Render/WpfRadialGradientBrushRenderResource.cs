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
    public class WpfRadialGradientBrushRenderResource : WpfGradientBrushRenderResource, IRadialGradientBrushRenderResource
    {
        private Point center;
        public Point Center
        {
            get { return center; }
            set
            {
                if (center == value)
                {
                    return;
                }

                center = value;
                brush.Center = converter.Convert(center);
            }
        }

        private Point gradientOrigin;
        public Point GradientOrigin
        {
            get { return gradientOrigin; }
            set
            {
                if (gradientOrigin == value)
                {
                    return;
                }

                gradientOrigin = value;
                brush.GradientOrigin = converter.Convert(gradientOrigin);
            }
        }

        private double radiusX;
        public double RadiusX
        {
            get { return radiusX; }
            set
            {
                if (radiusX == value)
                {
                    return;
                }

                radiusX = value;
                brush.RadiusX = radiusX;
            }
        }

        private double radiusY;
        public double RadiusY
        {
            get { return radiusY; }
            set
            {
                if (radiusY == value)
                {
                    return;
                }

                radiusY = value;
                brush.RadiusY = radiusY;
            }
        }

        private wpf::System.Windows.Media.RadialGradientBrush brush;
        private WpfValueConverter converter;

        public WpfRadialGradientBrushRenderResource(WpfValueConverter converter) :
            this(new wpf::System.Windows.Media.RadialGradientBrush(), converter)
        {
            //
        }

        public WpfRadialGradientBrushRenderResource(wpf::System.Windows.Media.RadialGradientBrush brush, WpfValueConverter converter) :
            base(brush, converter)
        {
            this.brush = brush;
            this.converter = converter;
        }
    }
}
