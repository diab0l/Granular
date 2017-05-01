using System;
using System.Windows;
using System.Windows.Media;
using Bridge.Html5;
using Granular.Host.Render;

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
    }
}
