using System;
using System.Windows;
using System.Windows.Media;
using Bridge.Html5;
using Granular.Host.Render;
using Granular.Extensions;

namespace Granular.Host
{
    public static class SvgElementExtensions
    {
        public static void SetSvgStartPoint(this HTMLElement element, Point startPoint, SvgValueConverter converter)
        {
            element.SetSvgPointAttributes("x1", "y1", startPoint, converter);
        }

        public static void SetSvgEndPoint(this HTMLElement element, Point endPoint, SvgValueConverter converter)
        {
            element.SetSvgPointAttributes("x2", "y2", endPoint, converter);
        }

        public static void SetSvgCenter(this HTMLElement element, Point center, SvgValueConverter converter)
        {
            element.SetSvgPointAttributes("cx", "cy", center, converter);
        }

        public static void SetSvgGradientOrigin(this HTMLElement element, Point gradientOrigin, SvgValueConverter converter)
        {
            element.SetSvgPointAttributes("fx", "fy", gradientOrigin, converter);
        }

        public static void SetSvgSpreadMethod(this HTMLElement element, GradientSpreadMethod spreadMethod, SvgValueConverter converter)
        {
            element.SetAttribute("spreadMethod", converter.ToSpreadMethodString(spreadMethod));
        }

        public static void SetSvgMappingMode(this HTMLElement element, BrushMappingMode mappingMode, SvgValueConverter converter)
        {
            element.SetAttribute("gradientUnits", converter.ToGradientUnitsString(mappingMode));
        }

        public static void SetSvgTransform(this HTMLElement element, Matrix transform, SvgValueConverter converter)
        {
            if (transform.IsNullOrIdentity())
            {
                element.RemoveAttribute("transform");
            }
            else
            {
                element.SetAttribute("transform", converter.ToMatrixString(transform));
            }
        }

        public static void SetSvgFill(this HTMLElement element, HtmlBrushRenderResource brush)
        {
            element.SetSvgBrush("fill", brush);
        }

        public static void SetSvgStroke(this HTMLElement element, HtmlBrushRenderResource brush)
        {
            element.SetSvgBrush("stroke", brush);
        }

        public static void SetSvgOpacity(this HTMLElement element, double opacity, SvgValueConverter converter)
        {
            element.SetSvgAttribute("opacity", opacity, converter);
        }

        public static void SetSvgStrokeThickness(this HTMLElement element, double strokeThickness, SvgValueConverter converter)
        {
            element.SetSvgAttribute("stroke-width", strokeThickness, converter);
        }

        public static void SetSvgGeometry(this HTMLElement element, HtmlGeometryRenderResource geometry)
        {
            if (geometry == null)
            {
                element.RemoveAttribute("d");
            }
            else
            {
                element.SetSvgAttribute("d", geometry.Data);
            }
        }

        private static void SetSvgAttribute(this HTMLElement element, string attributeName, double value, SvgValueConverter converter)
        {
            if (value.IsNaN())
            {
                element.RemoveAttribute(attributeName);
            }
            else
            {
                element.SetAttribute(attributeName, converter.ToImplicitValueString(value));
            }
        }

        private static void SetSvgAttribute(this HTMLElement element, string attributeName, string value)
        {
            if (value.IsNullOrEmpty())
            {
                element.RemoveAttribute(attributeName);
            }
            else
            {
                element.SetAttribute(attributeName, value);
            }
        }

        private static void SetSvgPointAttributes(this HTMLElement element, string xAttributeName, string yAttributeName, Point point, SvgValueConverter converter)
        {
            if (point.IsNullOrEmpty())
            {
                element.RemoveAttribute(xAttributeName);
                element.RemoveAttribute(yAttributeName);
            }
            else
            {
                element.SetAttribute(xAttributeName, converter.ToImplicitValueString(point.X));
                element.SetAttribute(yAttributeName, converter.ToImplicitValueString(point.Y));
            }
        }

        private static void SetSvgBrush(this HTMLElement element, string attributeName, HtmlBrushRenderResource brush)
        {
            if (brush == null)
            {
                element.RemoveAttribute(attributeName);
            }
            else
            {
                element.SetAttribute(attributeName, brush.Uri);
            }
        }
    }
}
