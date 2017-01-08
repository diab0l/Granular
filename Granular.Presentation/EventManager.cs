using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using Granular.Collections;

namespace System.Windows
{
    public static class EventManager
    {
        private sealed class RoutedEventHashKey
        {
            public Type Owner { get; private set; }
            public string Name { get; private set; }
            public string HashString { get; private set; }

            public RoutedEventHashKey(Type owner, string name)
            {
                this.Owner = owner;
                this.Name = name;
                this.HashString = owner.FullName + "," + name;
            }

            public override bool Equals(object obj)
            {
                RoutedEventHashKey other = obj as RoutedEventHashKey;

                return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                    Object.Equals(this.Owner, other.Owner) &&
                    Object.Equals(this.Name, other.Name);
            }

            public override int GetHashCode()
            {
                return Owner.GetHashCode() ^ Name.GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("{0}.{1}", Owner.FullName, Name);
            }
        }

        private static readonly ConvertedStringDictionary<RoutedEventHashKey, RoutedEvent> registeredEvents = new ConvertedStringDictionary<RoutedEventHashKey, RoutedEvent>(hashKey => hashKey.HashString);

        public static RoutedEvent RegisterRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            RoutedEventHashKey key = new RoutedEventHashKey(ownerType, name);

            if (registeredEvents.ContainsKey(key))
            {
                throw new Granular.Exception("RoutedEvent {0}.{1} is already registered", ownerType.Name, name);
            }

            RoutedEvent routedEvent = new RoutedEvent(name, routingStrategy, handlerType, ownerType);

            registeredEvents.Add(key, routedEvent);

            return routedEvent;
        }

        public static void AddOwner(RoutedEvent routedEvent, Type ownerType)
        {
            RoutedEventHashKey key = new RoutedEventHashKey(ownerType, routedEvent.Name);

            if (registeredEvents.ContainsKey(key))
            {
                throw new Granular.Exception("Type \"{0}\" is already an owner of RoutedEvent \"{1}\"", key.Owner.Name, routedEvent);
            }

            registeredEvents.Add(key, routedEvent);
        }

        public static void RegisterClassHandler(Type classType, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            routedEvent.RegisterClassHandler(classType, new RoutedEventHandlerItem(handler, handledEventsToo));
        }

        // Get event that is owned by containingType or one of its base classes
        public static RoutedEvent GetEvent(Type containingType, string eventName)
        {
            RoutedEvent routedEvent;

            while (containingType != null && containingType != typeof(Visual))
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(containingType.GetTypeHandle());

                if (registeredEvents.TryGetValue(new RoutedEventHashKey(containingType, eventName), out routedEvent))
                {
                    return routedEvent;
                }

                containingType = containingType.BaseType;
            }

            return null;
        }
    }
}
