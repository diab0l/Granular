extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfBorderRenderElement : IBorderRenderElement, IWpfRenderElement
    {
        private Brush background;
        public Brush Background
        {
            get { return background; }
            set
            {
                if (background == value)
                {
                    return;
                }

                background = value;
                border.Background = converter.Convert(background, factory);
            }
        }

        private Thickness borderThickness;
        public Thickness BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                border.BorderThickness = converter.Convert(borderThickness);
            }
        }

        private Brush borderBrush;
        public Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                if (borderBrush == value)
                {
                    return;
                }

                borderBrush = value;
                border.BorderBrush = converter.Convert(borderBrush, factory);
            }
        }

        private Rect bounds;
        public Rect Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                wpf::System.Windows.Controls.Canvas.SetLeft(border, bounds.Left);
                wpf::System.Windows.Controls.Canvas.SetTop(border, bounds.Top);
                border.Width = bounds.Width;
                border.Height = bounds.Height;
            }
        }

        private CornerRadius cornerRadius;
        public CornerRadius CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                border.CornerRadius = converter.Convert(cornerRadius);
            }
        }

        public bool IsHitTestVisible
        {
            get { return border.IsHitTestVisible; }
            set { border.IsHitTestVisible = value; }
        }

        private wpf::System.Windows.Controls.Border border;
        public wpf::System.Windows.FrameworkElement WpfElement { get { return border; } }

        private IRenderElementFactory factory;
        private WpfValueConverter converter;

        public WpfBorderRenderElement(IRenderElementFactory factory, WpfValueConverter converter)
        {
            this.factory = factory;
            this.converter = converter;
            border = new wpf::System.Windows.Controls.Border();
        }
    }
}
