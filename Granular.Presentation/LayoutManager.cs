using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows
{
    public class LayoutManager
    {
        public static readonly LayoutManager Current = new LayoutManager();

        private HashSet<UIElement> measureQueue;
        private HashSet<UIElement> arrangeQueue;

        private DispatcherOperation updateLayoutOperation;

        private LayoutManager()
        {
            measureQueue = new HashSet<UIElement>();
            arrangeQueue = new HashSet<UIElement>();
        }

        public void AddMeasure(UIElement element)
        {
            measureQueue.Add(element);
            BeginUpdateLayout();
        }

        public void RemoveMeasure(UIElement element)
        {
            measureQueue.Remove(element);
        }

        public void AddArrange(UIElement element)
        {
            arrangeQueue.Add(element);
            BeginUpdateLayout();
        }

        public void RemoveArrange(UIElement element)
        {
            arrangeQueue.Remove(element);
        }

        public void BeginUpdateLayout()
        {
            if (updateLayoutOperation == null || updateLayoutOperation.Status == DispatcherOperationStatus.Completed)
            {
                updateLayoutOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Action)UpdateLayout);
            }
        }

        public void UpdateLayout()
        {
            HashSet<UIElement> arrangedElements = new HashSet<UIElement>();

            while (measureQueue.Count > 0 || arrangeQueue.Count > 0)
            {
                while (measureQueue.Count > 0)
                {
                    UIElement element = GetTopElement(measureQueue);
                    element.Measure(element.VisualParent == null || element.PreviousAvailableSize.IsEmpty ? Size.Infinity : element.PreviousAvailableSize);
                }

                arrangedElements.AddRange(arrangeQueue.SelectMany(element => GetElementPath(element)));

                while (arrangeQueue.Count > 0)
                {
                    UIElement element = GetTopElement(arrangeQueue);
                    element.Arrange(element.VisualParent == null || element.PreviousFinalRect.IsEmpty ? new Rect(element.DesiredSize) : element.PreviousFinalRect);
                }

                while (arrangedElements.Count > 0 && measureQueue.Count == 0 && arrangeQueue.Count == 0) // LayoutUpdated can invalidate other elements
                {
                    UIElement element = arrangedElements.First();
                    arrangedElements.Remove(element);

                    element.RaiseLayoutUpdated();
                }
            }
        }

        private UIElement GetTopElement(HashSet<UIElement> measureQueue)
        {
            UIElement topElement = null;

            foreach (UIElement element in measureQueue)
            {
                if (topElement == null || topElement.VisualLevel > element.VisualLevel)
                {
                    topElement = element;
                }
            }

            return topElement;
        }

        private static IEnumerable<UIElement> GetElementPath(UIElement element)
        {
            while (element != null)
            {
                yield return element;
                element = (UIElement)element.VisualParent;
            }
        }
    }
}
