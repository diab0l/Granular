using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

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
        private bool isRenderScheduled;

        private RenderQueue()
        {
            items = new List<IRenderItem>();
        }

        public void Add(IRenderItem item)
        {
            items.Add(item);

            if (!isRenderScheduled)
            {
                isRenderScheduled = true;
                Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                {
                    isRenderScheduled = false;

                    IEnumerable<IRenderItem> currentItems = items.ToArray();
                    items.Clear();

                    System.Html.Window.RequestAnimationFrame(time => Render(currentItems));
                }, DispatcherPriority.Render);
            }
        }

        private static void Render(IEnumerable<IRenderItem> items)
        {
            foreach (IRenderItem item in items)
            {
                item.Render();
            }
        }
    }
}
