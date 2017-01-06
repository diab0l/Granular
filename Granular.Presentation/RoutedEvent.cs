using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;

namespace System.Windows
{
    public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);

    public class RoutedEventArgs : EventArgs
    {
        public RoutedEvent RoutedEvent { get; private set; }
        public object OriginalSource { get; private set; }

        public object Source { get; set; }
        public bool Handled { get; set; }

        public RoutedEventArgs(RoutedEvent routedEvent, object originalSource)
        {
            this.RoutedEvent = routedEvent;
            this.OriginalSource = originalSource;
        }

        public virtual void InvokeEventHandler(Delegate handler, object target)
        {
            if (!(handler is RoutedEventHandler))
            {
                throw new Granular.Exception("Can't dynamically invoke a non RoutedEventHandler, \"{0}\" must override InvokeEventHandler", GetType().Name);
            }

            ((RoutedEventHandler)handler)(target, this);
        }
    }

    public class RoutedEventHandlerItem
    {
        public Delegate Handler { get; private set; }
        public bool HandledEventsToo { get; private set; }

        public RoutedEventHandlerItem(Delegate handler, bool handledEventsToo)
        {
            this.Handler = handler;
            this.HandledEventsToo = handledEventsToo;
        }
    }

    public enum RoutingStrategy
    {
        Tunnel,
        Bubble,
        Direct
    }

    [TypeConverter(typeof(RoutedEventTypeConverter))]
    public sealed class RoutedEvent
    {
        public string Name { get; private set; }
        public RoutingStrategy RoutingStrategy { get; private set; }
        public Type HandlerType { get; private set; }
        public Type OwnerType { get; private set; }
        public string StringKey { get; private set; }

        private ListDictionary<Type, RoutedEventHandlerItem> classesHandlers;

        public RoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            this.Name = name;
            this.RoutingStrategy = routingStrategy;
            this.HandlerType = handlerType;
            this.OwnerType = ownerType;
            this.StringKey = ownerType.FullName + "," + name;
        }

        public RoutedEvent AddOwner(Type ownerType)
        {
            EventManager.AddOwner(this, ownerType);
            return this;
        }

        public override int GetHashCode()
        {
            return OwnerType.GetHashCode() ^ Name.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", OwnerType.FullName, Name);
        }

        public void RegisterClassHandler(Type classType, RoutedEventHandlerItem routedEventHandlerItem)
        {
            if (classesHandlers == null)
            {
                classesHandlers = new ListDictionary<Type, RoutedEventHandlerItem>();
            }

            classesHandlers.Add(classType, routedEventHandlerItem);
        }

        public IEnumerable<RoutedEventHandlerItem> GetClassHandlers(Type classType)
        {
            if (classesHandlers == null)
            {
                return new RoutedEventHandlerItem[0];
            }

            IEnumerable <RoutedEventHandlerItem> flattenedHandlers = null;
            int classesHandlesCount = 0;

            while (classType != null)
            {
                IEnumerable<RoutedEventHandlerItem> classHandlers = classesHandlers.GetValues(classType);

                if (classHandlers.Any())
                {
                    flattenedHandlers = flattenedHandlers != null ? classHandlers.Concat(flattenedHandlers) : classHandlers;
                    classesHandlesCount++;
                }

                classType = classType.BaseType;
            }

            return classesHandlesCount > 1 ? flattenedHandlers.ToArray() : (flattenedHandlers ?? new RoutedEventHandlerItem[0]);
        }
    }

    public class RoutedEventTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            string text = value.ToString().Trim();

            XamlName eventName = XamlName.FromPrefixedName(text, namespaces);
            Type containingType = eventName.ResolveContainingType(null);

            if (containingType == null)
            {
                throw new Granular.Exception("Invalid routed event name \"{0}\"", eventName.LocalName);
            }

            RoutedEvent routedEvent = EventManager.GetEvent(containingType, eventName.MemberName);

            if (routedEvent == null)
            {
                throw new Granular.Exception("Can't find a routed event named \"{0}\"", eventName);
            }

            return routedEvent;
        }
    }
}
