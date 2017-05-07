using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media
{
    public class StreamGeometry : Geometry
    {
        private string source;

        public StreamGeometry(string source)
        {
            this.source = source;
        }

        protected override string GetRenderResourceData()
        {
            return source;
        }
    }
}