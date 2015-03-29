using System;
using System.Collections.Generic;
using System.Text;

namespace Granular.Host
{
    public interface IRenderItem
    {
        void Render();
    }

    public interface IRenderQueue
    {
        void Add(IRenderItem item);
    }

    public class RenderQueue : IRenderQueue
    {
        public static readonly RenderQueue Default = new RenderQueue();

        private List<IRenderItem> items;

        private RenderQueue()
        {
            items = new List<IRenderItem>();
        }

        public void Add(IRenderItem item)
        {
            items.Add(item);

            if (items.Count == 1)
            {
                System.Html.Window.RequestAnimationFrame(time => Render());
            }
        }

        private void Render()
        {
            IEnumerable<IRenderItem> currentItems = items.ToArray();
            items.Clear();

            foreach (IRenderItem item in currentItems)
            {
                item.Render();
            }
        }
    }
}
