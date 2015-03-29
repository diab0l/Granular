using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

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

        public RoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            this.Name = name;
            this.RoutingStrategy = routingStrategy;
            this.HandlerType = handlerType;
            this.OwnerType = ownerType;
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
    }

    public class RoutedEventTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            string text = value.ToString().Trim();

            XamlName eventName = XamlName.FromPrefixedName(text, namespaces);

            if (!eventName.IsMemberName)
            {
                throw new Granular.Exception("Invalid routed event name \"{0}\"", eventName.LocalName);
            }

            Type containingType = TypeParser.ParseType(eventName.ContainingTypeName);

            RoutedEvent routedEvent = EventManager.FindRoutedEvent(containingType, eventName.MemberName);

            if (routedEvent == null)
            {
                throw new Granular.Exception("Can't find a routed event named \"{0}\"", eventName);
            }

            return routedEvent;
        }
    }
}
