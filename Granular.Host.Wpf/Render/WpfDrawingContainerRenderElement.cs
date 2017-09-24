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
        private Geometry clip;
        public Geometry Clip
        {
            get { return clip; }
            set
            {
                if (clip == value)
                {
                    return;
                }

                clip = value;
                WpfElement.Clip = converter.Convert(clip, factory);
            }
        }

        public double Opacity
        {
            get { return WpfElement.Opacity; }
            set { WpfElement.Opacity = value; }
        }

        private Transform transform;
        public Transform Transform
        {
            get { return transform; }
            set
            {
                if (transform == value)
                {
                    return;
                }

                transform = value;
                WpfElement.RenderTransform = converter.Convert(transform, factory);
            }
        }

        private IRenderElementFactory factory;
        private WpfValueConverter converter;

        public WpfDrawingContainerRenderElement(IRenderElementFactory factory, WpfValueConverter converter) :
            base(new wpf::System.Windows.Controls.Canvas())
        {
            this.factory = factory;
            this.converter = converter;
        }
    }
}
