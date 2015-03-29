using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Host.Tests.Web
{
    public class TestRenderQueue : IRenderQueue
    {
        public List<IRenderItem> Items { get; private set; }

        public TestRenderQueue()
        {
            Items = new List<IRenderItem>();
        }

        public void Add(IRenderItem item)
        {
            Items.Add(item);
        }

        public void Render()
        {
            IEnumerable<IRenderItem> currentItems = Items.ToArray();
            Items.Clear();

            foreach (IRenderItem item in currentItems)
            {
                item.Render();
            }
        }
    }
}
