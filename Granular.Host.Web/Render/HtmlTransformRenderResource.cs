using Granular.Extensions;
using System;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlTransformRenderResource : ITransformRenderResource
    {
        public event EventHandler MatrixChanged;
        private Matrix matrix;
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
                MatrixChanged.Raise(this);
            }
        }
    }
}