using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Granular.Host
{
    public class RenderQueue
    {
        public static readonly RenderQueue Default = new RenderQueue();

        private List<Action> actions;
        private bool isRenderScheduled;

        private RenderQueue()
        {
            actions = new List<Action>();
        }

        public void InvokeAsync(Action action)
        {
            actions.Add(action);
            RequestAnimationFrame();
        }

        private void RequestAnimationFrame()
        {
            if (isRenderScheduled)
            {
                return;
            }

            isRenderScheduled = true;
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                isRenderScheduled = false;
                List<Action> currentActions = actions;
                actions = new List<Action>();

                foreach (Action action in currentActions)
                {
                    action();
                }
            }, DispatcherPriority.Background);
        }
    }
}
