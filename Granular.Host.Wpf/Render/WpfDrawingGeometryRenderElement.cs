extern alias wpf;

using System;
using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfDrawingGeometryRenderElement : IDrawingGeometryRenderElement, IWpfRenderElement
    {
        public wpf::System.Windows.FrameworkElement WpfElement { get { return element; } }

        private Brush fill;
        public Brush Fill
        {
            get { return fill; }
            set
            {
                if (fill == value)
                {
                    return;
                }

                fill = value;
                element.Fill = converter.Convert(fill, factory);
            }
        }

        private Brush stroke;
        public Brush Stroke
        {
            get { return stroke; }
            set
            {
                if (stroke == value)
                {
                    return;
                }

                stroke = value;
                element.Stroke = converter.Convert(stroke, factory);
            }
        }

        private double strokeThickness;
        public double StrokeThickness
        {
            get { return strokeThickness; }
            set
            {
                if (strokeThickness == value)
                {
                    return;
                }

                strokeThickness = value;
                element.StrokeThickness = strokeThickness;
            }
        }

        private Geometry geometry;
        public Geometry Geometry
        {
            get { return geometry; }
            set
            {
                if (geometry == value)
                {
                    return;
                }

                geometry = value;
                element.Data = converter.Convert(geometry, factory);
            }
        }

        private wpf::System.Windows.Shapes.Path element;
        private IRenderElementFactory factory;
        private WpfValueConverter converter;

        public WpfDrawingGeometryRenderElement(WpfRenderElementFactory factory, WpfValueConverter converter)
        {
            this.factory = factory;
            this.converter = converter;
            this.element = new wpf::System.Windows.Shapes.Path();
        }
    }
}