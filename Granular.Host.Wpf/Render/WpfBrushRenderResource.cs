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
    public interface IWpfBrushRenderResource
    {
        wpf::System.Windows.Media.Brush WpfResource { get; }
    }

    public abstract class WpfBrushRenderResource : IBrushRenderResource, IWpfBrushRenderResource
    {
        public wpf::System.Windows.Media.Brush WpfResource { get { return brush; } }

        public double Opacity
        {
            get { return brush.Opacity; }
            set { brush.Opacity = value; }
        }

        private wpf::System.Windows.Media.Brush brush;

        public WpfBrushRenderResource(wpf::System.Windows.Media.Brush brush)
        {
            this.brush = brush;
        }
    }
}