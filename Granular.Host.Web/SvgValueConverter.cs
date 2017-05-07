using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Granular.Host
{
    public class SvgValueConverter
    {
        public string ToImplicitValueString(double value)
        {
            return Math.Round(value, 2).ToString();
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
    }
}
