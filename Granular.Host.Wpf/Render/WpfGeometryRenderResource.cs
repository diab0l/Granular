extern alias wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfGeometryRenderResource : IGeometryRenderResource
    {
        public wpf::System.Windows.Media.Geometry Geometry { get { return geometryGroup; } }

        private Transform transform;
        public Transform Transform
        {
            get { return transform; }
            set
            {
                if (transform == value)
                {
                    return;
                }

                transform = value;
                geometryGroup.Transform = ((WpfTransformRenderResource)transform.GetRenderResource(factory)).Transform;
            }
        }

        private string data;
        public string Data
        {
            get { return data; }
            set
            {
                if (data == value)
                {
                    return;
                }

                data = value;
                geometryGroup.Children.Clear();
                geometryGroup.Children.Add(wpf::System.Windows.Media.Geometry.Parse(data));
            }
        }

        private WpfRenderElementFactory factory;
        private WpfValueConverter converter;
        private wpf::System.Windows.Media.GeometryGroup geometryGroup;

        public WpfGeometryRenderResource(WpfRenderElementFactory factory, WpfValueConverter converter)
        {
            this.factory = factory;
            this.converter = converter;

            geometryGroup = new wpf::System.Windows.Media.GeometryGroup();
        }
    }
}
