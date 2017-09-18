extern alias wpf;

using System;
using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    internal class WpfDrawingImageElement : IDrawingImageRenderElement, IWpfRenderElement
    {
        public wpf::System.Windows.FrameworkElement WpfElement { get { return element; } }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                if (imageSource == value)
                {
                    return;
                }

                imageSource = value;
                element.Source = converter.Convert(imageSource, factory);
            }
        }

        private Rect rectangle;
        public Rect Rectangle
        {
            get { return rectangle; }
            set
            {
                if (rectangle == value)
                {
                    return;
                }

                rectangle = value;
                wpf::System.Windows.Controls.Canvas.SetLeft(element, rectangle.Left);
                wpf::System.Windows.Controls.Canvas.SetTop(element, rectangle.Top);
                element.Width = rectangle.Width;
                element.Height = rectangle.Height;
            }
        }

        private wpf::System.Windows.Controls.Image element;
        private IRenderElementFactory factory;
        private WpfValueConverter converter;

        public WpfDrawingImageElement(WpfRenderElementFactory factory, WpfValueConverter converter)
        {
            this.factory = factory;
            this.converter = converter;
            this.element = new wpf::System.Windows.Controls.Image { Stretch = wpf::System.Windows.Media.Stretch.Fill };
        }
    }
}