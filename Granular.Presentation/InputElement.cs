using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public interface IInputElement
    {
        void RaiseEvent(RoutedEventArgs e);
        Point GetRelativePosition(Point absolutePosition);
        IEnumerable<IInputElement> GetPathFromRoot();
    }

    public static class IInputElementExtensions
    {
        public static bool RaiseEvents(this IInputElement element, RoutedEventArgs previewEventArgs, RoutedEventArgs eventArgs)
        {
            element.RaiseEvent(previewEventArgs);

            eventArgs.Handled = previewEventArgs.Handled;
            element.RaiseEvent(eventArgs);

            return eventArgs.Handled;
        }
    }
}
