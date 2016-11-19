using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;
using Granular.Collections;

namespace System.Windows.Markup
{
    public interface IEventAdapter
    {
        Type HandlerType { get; }
        void AddHandler(object target, Delegate handler);
    }

    public static class EventAdapter
    {
        private static CacheDictionary<TypeMemberKey, IEventAdapter> adaptersCache = new CacheDictionary<TypeMemberKey, IEventAdapter>(TryCreateAdapter);

        public static IEventAdapter CreateAdapter(Type targetType, XamlName eventName)
        {
            IEventAdapter eventAdapter;
            return adaptersCache.TryGetValue(new TypeMemberKey(targetType, eventName), out eventAdapter) ? eventAdapter : null;
        }

        private static bool TryCreateAdapter(TypeMemberKey key, out IEventAdapter adapter)
        {
            adapter = null;

            RoutedEvent routedEvent = GetRoutedEvent(key.Type, key.MemberName);
            if (routedEvent != null)
            {
                adapter = new RoutedEventAdapter(routedEvent);
                return true;
            }

            EventInfo clrEvent = GetClrEvent(key.Type, key.MemberName);
            if (clrEvent != null)
            {
                adapter = new ClrEventAdapter(clrEvent);
                return true;
            }

            PropertyInfo eventProperty = GetEventProperty(key.Type, key.MemberName);
            if (eventProperty != null)
            {
                adapter = new EventPropertyAdapter(eventProperty);
                return true;
            }

            return false;
        }

        private static RoutedEvent GetRoutedEvent(Type containingType, XamlName eventName)
        {
            string eventMemberName = eventName.MemberName;
            Type eventContainingType = eventName.IsMemberName ? TypeParser.ParseType(eventName.ContainingTypeName) : containingType;

            return EventManager.FindRoutedEvent(containingType, eventMemberName);
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
