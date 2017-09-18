using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Granular.Host.Render;
using System.Windows;
using Granular.Extensions;
using System.Windows.Controls;

namespace Granular.Host
{
    public class SvgValueConverter
    {
        public string ToImplicitValueString(double value)
        {
            return Math.Round(value, 2).ToString();
        }

        public string ToPixelString(double size)
        {
            return String.Format("{0}px", size);
        }

        public string ToColorString(Color color)
        {
            return String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }

        public string ToSpreadMethodString(GradientSpreadMethod spreadMethod)
        {
            switch (spreadMethod)
            {
                case GradientSpreadMethod.Pad: return "pad";
                case GradientSpreadMethod.Reflect: return "reflect";
                case GradientSpreadMethod.Repeat: return "repeat";
            }

            throw new Granular.Exception("Unexpected GradientSpreadMethod \"{0}\"", spreadMethod);
        }

        public string ToPathDataString(Geometry geometry, IRenderElementFactory factory)
        {
            return ((HtmlGeometryRenderResource)geometry.GetRenderResource(factory)).Data;
        }

        public string ToImageUrl(ImageSource imageSource, IRenderElementFactory factory)
        {
            return ((HtmlImageSourceRenderResource)imageSource.GetRenderResource(factory)).Url;
        }

        public string ToMatrixString(Matrix matrix)
        {
            return String.Format("matrix({0}, {1}, {2}, {3}, {4}, {5})", ToImplicitValueString(matrix.M11), ToImplicitValueString(matrix.M12), ToImplicitValueString(matrix.M21), ToImplicitValueString(matrix.M22), ToImplicitValueString(matrix.OffsetX), ToImplicitValueString(matrix.OffsetY));
        }

        public string ToGradientUnitsString(BrushMappingMode mappingMode)
        {
            switch (mappingMode)
            {
                case BrushMappingMode.Absolute: return "userSpaceOnUse";
                case BrushMappingMode.RelativeToBoundingBox: return "objectBoundingBox";
            }

            throw new Granular.Exception("Unexpected BrushMappingMode \"{0}\"", mappingMode);
        }

        public string ToGradientTransformString(double radiusX, double radiusY)
        {
            return $"matrix({radiusX * 2},0,0,{radiusY * 2},{0.5 - radiusX},{0.5 - radiusY})";
        }

        public string ToFontStyleString(FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case FontStyle.Normal: return "normal";
                case FontStyle.Italic: return "italic";
                case FontStyle.Oblique: return "oblique";
            }

            throw new Granular.Exception("Unexpected FontStyle \"{0}\"", fontStyle);
        }

        public string ToFontStretchString(FontStretch fontStretch)
        {
            switch (fontStretch)
            {
                case FontStretch.UltraCondensed: return "ultra-condensed";
                case FontStretch.ExtraCondensed: return "extra-condensed";
                case FontStretch.Condensed: return "condensed";
                case FontStretch.SemiCondensed: return "semi-condensed";
                case FontStretch.Medium:
                case FontStretch.Normal: return "normal";
                case FontStretch.SemiExpanded: return "semi-expanded";
                case FontStretch.Expanded: return "expanded";
                case FontStretch.ExtraExpanded: return "extra-expanded";
                case FontStretch.UltraExpanded: return "ultra-expanded";
            }

            throw new Granular.Exception("Unexpected FontStretch \"{0}\"", fontStretch);
        }

        public string ToFontFamilyNamesString(FontFamily fontFamily)
        {
            return fontFamily.FamilyNames.Select(familyName => String.Format("\"{0}\"", familyName)).Aggregate((s1, s2) => String.Format("{0}, {1}", s1, s2));
        }

        public string ToFontWeightString(FontWeight fontWeight)
        {
            switch (fontWeight)
            {
                case FontWeight.Thin: return "100";
                case FontWeight.ExtraLight:
                case FontWeight.UltraLight: return "200";
                case FontWeight.Light: return "300";
                case FontWeight.Normal:
                case FontWeight.Regular: return "400";
                case FontWeight.Medium: return "500";
                case FontWeight.DemiBold:
                case FontWeight.SemiBold: return "600";
                case FontWeight.Bold: return "700";
                case FontWeight.ExtraBold:
                case FontWeight.UltraBold: return "800";
                case FontWeight.Black:
                case FontWeight.Heavy: return "900";
                case FontWeight.ExtraBlack:
                case FontWeight.UltraBlack: return "950";
            }

            throw new Granular.Exception("Unexpected FontWeight \"{0}\"", fontWeight);
        }
    }
}
