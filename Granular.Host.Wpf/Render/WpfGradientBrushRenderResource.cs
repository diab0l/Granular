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
    public abstract class WpfGradientBrushRenderResource : WpfBrushRenderResource, IGradientBrushRenderResource
    {
        private GradientSpreadMethod spreadMethod;
        public GradientSpreadMethod SpreadMethod
        {
            get { return spreadMethod; }
            set
            {
                if (spreadMethod == value)
                {
                    return;
                }

                spreadMethod = value;
                brush.SpreadMethod = converter.Convert(spreadMethod);
            }
        }

        private BrushMappingMode mappingMode = BrushMappingMode.RelativeToBoundingBox;
        public BrushMappingMode MappingMode
        {
            get { return mappingMode; }
            set
            {
                if (mappingMode == value)
                {
                    return;
                }

                mappingMode = value;
                brush.MappingMode = converter.Convert(mappingMode);
            }
        }

        private IEnumerable<RenderGradientStop> gradientStops;
        public IEnumerable<RenderGradientStop> GradientStops
        {
            get { return gradientStops; }
            set
            {
                if (gradientStops == value)
                {
                    return;
                }

                gradientStops = value;
                brush.GradientStops = new wpf::System.Windows.Media.GradientStopCollection(gradientStops.Select(stop => new wpf::System.Windows.Media.GradientStop(converter.Convert(stop.Color), stop.Offset)).ToArray());
            }
        }

        private wpf::System.Windows.Media.GradientBrush brush;
        private WpfValueConverter converter;

        public WpfGradientBrushRenderResource(wpf::System.Windows.Media.GradientBrush brush, WpfValueConverter converter) :
            base(brush)
        {
            this.brush = brush;
            this.converter = converter;
        }
    }
}
