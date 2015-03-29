using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xaml;
using Granular.Extensions;

namespace System.Windows.Markup
{
    public interface IEventAdapter
    {
        Type HandlerType { get; }
        void AddHandler(object target, Delegate handler);
    }

    public static class EventAdapter
    {
        public static IEventAdapter CreateAdapter(Type targetType, XamlName eventName)
        {
            RoutedEvent routedEvent = GetRoutedEvent(targetType, eventName);
            if (routedEvent != null)
            {
                return new RoutedEventAdapter(routedEvent);
            }

            EventInfo clrEvent = GetClrEvent(targetType, eventName);
            if (clrEvent != null)
            {
                return new ClrEventAdapter(clrEvent);
            }

            PropertyInfo eventProperty = GetEventProperty(targetType, eventName);
            if (eventProperty != null)
            {
                return new EventPropertyAdapter(eventProperty);
            }

            return null;
        }

        private static RoutedEvent GetRoutedEvent(Type containingType, XamlName eventName)
        {
            string eventMemberName = eventName.MemberName;
            Type eventContainingType = eventName.IsMemberName ? TypeParser.ParseType(eventName.ContainingTypeName) : containingType;

            return EventManager.GetOwnedRoutedEvent(containingType, eventMemberName);
        }

        private static EventInfo GetClrEvent(Type containingType, XamlName eventName)
        {
            string eventMemberName = eventName.MemberName;
            Type eventContainingType = eventName.IsMemberName ? TypeParser.ParseType(eventName.ContainingTypeName) : containingType;

            return eventContainingType.GetEvent(eventMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        }

        private static PropertyInfo GetEventProperty(Type containingType, XamlName eventName)
        {
            string eventMemberName = eventName.MemberName;
            Type eventContainingType = eventName.IsMemberName ? TypeParser.ParseType(eventName.ContainingTypeName) : containingType;

            PropertyInfo eventProperty = eventContainingType.GetInstanceProperty(eventMemberName);
            return eventProperty != null && eventProperty.IsDelegate() ? eventProperty : null;
        }
    }

    public class RoutedEventAdapter : IEventAdapter
    {
        public Type HandlerType { get { return routedEvent.HandlerType; } }

        private RoutedEvent routedEvent;

        public RoutedEventAdapter(RoutedEvent routedEvent)
        {
            this.routedEvent = routedEvent;
        }

        public void AddHandler(object target, Delegate handler)
        {
            ((UIElement)target).AddHandler(routedEvent, handler);
        }
    }

    public class ClrEventAdapter : IEventAdapter
    {
        public Type HandlerType { get { return eventInfo.GetEventHandlerType(); } }

        private EventInfo eventInfo;

        public ClrEventAdapter(EventInfo eventInfo)
        {
            this.eventInfo = eventInfo;
        }

        public void AddHandler(object target, Delegate handler)
        {
            eventInfo.AddEventHandler(target, handler);
        }
    }

    public class EventPropertyAdapter : IEventAdapter
    {
        public Type HandlerType { get { return eventProperty.PropertyType; } }

        private PropertyInfo eventProperty;

        public EventPropertyAdapter(PropertyInfo eventProperty)
        {
            this.eventProperty = eventProperty;
        }

        public void AddHandler(object target, Delegate handler)
        {
            eventProperty.SetValue(target, handler, new object[0]);
        }
    }
}
