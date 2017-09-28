extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host.Wpf.Render
{
    public class WpfVisualRenderElement : WpfContainerRenderElement, System.Windows.Media.IVisualRenderElement
    {
        private System.Windows.Media.Brush background;
        public System.Windows.Media.Brush Background
        {
            get { return background; }
            set
            {
                if (background == value)
                {
                    return;
                }

                background = value;
                container.Background = converter.Convert(background, factory);
            }
        }

        private System.Windows.Rect bounds;
        public System.Windows.Rect Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds == value)
                {
                    return;
                }

                bounds = value;

                wpf::System.Windows.Controls.Canvas.SetLeft(container, bounds.Left);
                wpf::System.Windows.Controls.Canvas.SetTop(container, bounds.Top);
                container.Width = bounds.Width;
                container.Height = bounds.Height;
            }
        }

        private System.Windows.Media.Geometry clip;
        public System.Windows.Media.Geometry Clip
        {
            get { return clip; }
            set
            {
                if (clip == value)
                {
                    return;
                }

                clip = value;
                container.Clip = converter.Convert(clip, factory);
            }
        }

        public bool ClipToBounds
        {
            get { return container.ClipToBounds; }
            set { container.ClipToBounds = value; }
        }

        private bool isHitTestVisible;
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                isHitTestVisible = value;
                container.IsHitTestVisible = isHitTestVisible;
            }
        }

        public bool IsVisible
        {
            get { return container.Visibility == wpf::System.Windows.Visibility.Visible; }
            set { container.Visibility = value ? wpf::System.Windows.Visibility.Visible : wpf::System.Windows.Visibility.Collapsed; }
        }

        public double Opacity
        {
            get { return container.Opacity; }
            set { container.Opacity = value; }
        }

        private System.Windows.Media.Matrix transform;
        public System.Windows.Media.Matrix Transform
        {
            get { return transform; }
            set
            {
                if (transform == value)
                {
                    return;
                }

                transform = value;
                container.RenderTransform = new wpf::System.Windows.Media.MatrixTransform(converter.Convert(transform));
            }
        }

        private wpf::System.Windows.Controls.Canvas container;
        private IRenderElementFactory factory;
        private WpfValueConverter converter;

        public WpfVisualRenderElement(object owner, IRenderElementFactory factory, WpfValueConverter converter) :
            this(CreateWpfElement(owner), factory, converter)
        {
            //
        }

        private WpfVisualRenderElement(wpf::System.Windows.Controls.Canvas container, IRenderElementFactory factory, WpfValueConverter converter) :
            base(container)
        {
            this.container = container;
            this.factory = factory;
            this.converter = converter;
        }

        private static wpf::System.Windows.Controls.Canvas CreateWpfElement(object owner)
        {
            return new wpf::System.Windows.Controls.Canvas
            {
                Name = owner.GetType().Name,
                DataContext = owner
            };
        }
    }
}
