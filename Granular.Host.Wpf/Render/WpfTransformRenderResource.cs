extern alias wpf;

using System;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfTransformRenderResource : ITransformRenderResource
    {
        public wpf::System.Windows.Media.Transform Transform { get { return matrixTransform; } }

        private Matrix matrix = Matrix.Identity;
        public Matrix Matrix
        {
            get { return matrix; }
            set
            {
                if (matrix == value)
                {
                    return;
                }

                matrix = value;
                matrixTransform.Matrix = converter.Convert(matrix);
            }
        }

        private WpfValueConverter converter;
        private wpf::System.Windows.Media.MatrixTransform matrixTransform;

        public WpfTransformRenderResource(WpfValueConverter converter)
        {
            this.converter = converter;
            matrixTransform = new wpf::System.Windows.Media.MatrixTransform();
        }
    }
}