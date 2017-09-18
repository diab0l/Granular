extern alias wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host.Wpf.Render
{
    public class WpfDrawingContainerRenderElement : WpfContainerRenderElement, IDrawingContainerRenderElement
    {
        public double Opacity
        {
            get { return WpfElement.Opacity; }
            set { WpfElement.Opacity = value; }
        }

        public WpfDrawingContainerRenderElement(WpfValueConverter converter) :
            base(new wpf::System.Windows.Controls.Canvas())
        {
            //
        }
    }
}
