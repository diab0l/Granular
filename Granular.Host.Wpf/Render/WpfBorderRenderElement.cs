extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Host.Wpf.Render
{
    public class WpfBorderRenderElement : System.Windows.Media.IBorderRenderElement, IWpfRenderElement
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

                if (background != null)
                {
                    background.Changed -= OnBackgroundChanged;
                }

                background = value;
                border.Background = converter.Convert(background);

                if (background != null)
                {
                    background.Changed += OnBackgroundChanged;
                }
            }
        }

        private System.Windows.Thickness borderThickness;
        public System.Windows.Thickness BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                border.BorderThickness = converter.Convert(borderThickness);
            }
        }

        private System.Windows.Media.Brush borderBrush;
        public System.Windows.Media.Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                if (borderBrush == value)
                {
                    return;
                }

                if (borderBrush != null)
                {
                    borderBrush.Changed -= OnBorderBrushChanged;
                }

                borderBrush = value;
                border.BorderBrush = converter.Convert(borderBrush);

                if (borderBrush != null)
                {
                    borderBrush.Changed += OnBorderBrushChanged;
                }
            }
        }

        private System.Windows.Rect bounds;
        public System.Windows.Rect Bounds
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

        private System.Windows.CornerRadius cornerRadius;
        public System.Windows.CornerRadius CornerRadius
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

        private IWpfValueConverter converter;

        public WpfBorderRenderElement(IWpfValueConverter converter)
        {
            this.converter = converter;
            border = new wpf::System.Windows.Controls.Border();
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            border.Background = converter.Convert(Background);
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            border.BorderBrush = converter.Convert(BorderBrush);
        }
    }
}
