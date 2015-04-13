using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Collections;

namespace System.Windows
{
    public static class EventManager
    {
        private sealed class RoutedEventKey
        {
            public Type Owner { get; private set; }
            public string Name { get; private set; }

            public RoutedEventKey(Type owner, string name)
            {
                this.Owner = owner;
                this.Name = name;
            }

            public override bool Equals(object obj)
            {
                RoutedEventKey other = obj as RoutedEventKey;

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

        private sealed class ClassHandlerKey
        {
            public Type ClassType { get; private set; }
            public RoutedEvent RoutedEvent { get; private set; }

            public ClassHandlerKey(Type classType, RoutedEvent routedEvent)
            {
                this.ClassType = classType;
                this.RoutedEvent = routedEvent;
            }

            public override bool Equals(object obj)
            {
                ClassHandlerKey other = obj as ClassHandlerKey;

                return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                    Object.Equals(this.ClassType, other.ClassType) &&
                    Object.Equals(this.RoutedEvent, other.RoutedEvent);
            }

            public override int GetHashCode()
            {
                return ClassType.GetHashCode() ^ RoutedEvent.GetHashCode();
            }
        }

        private static readonly Dictionary<RoutedEventKey, RoutedEvent> registeredRoutedEvents = new Dictionary<RoutedEventKey, RoutedEvent>();
        private static readonly ListDictionary<ClassHandlerKey, RoutedEventHandlerItem> classHandlers = new ListDictionary<ClassHandlerKey, RoutedEventHandlerItem>();
        private static readonly CacheDictionary<ClassHandlerKey, IEnumerable<RoutedEventHandlerItem>> flattenedClassHandlersCache = new CacheDictionary<ClassHandlerKey, IEnumerable<RoutedEventHandlerItem>>(ResolveFlattenedClassHandlers);

        public static RoutedEvent RegisterRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            RoutedEventKey key = new RoutedEventKey(ownerType, name);

            if (registeredRoutedEvents.ContainsKey(key))
            {
                throw new Granular.Exception("RoutedEvent {0}.{1} is already registered", ownerType.Name, name);
            }

            RoutedEvent routedEvent = new RoutedEvent(name, routingStrategy, handlerType, ownerType);

            registeredRoutedEvents.Add(key, routedEvent);

            return routedEvent;
        }

        public static void AddOwner(RoutedEvent routedEvent, Type ownerType)
        {
            RoutedEventKey key = new RoutedEventKey(ownerType, routedEvent.Name);

            if (registeredRoutedEvents.ContainsKey(key))
            {
                throw new Granular.Exception("Type \"{0}\" is already an owner of RoutedEvent \"{1}\"", key.Owner.Name, routedEvent);
            }

            registeredRoutedEvents.Add(key, routedEvent);
        }

        public static void RegisterClassHandler(Type classType, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            RegisterClassHandler(classType, routedEvent, new RoutedEventHandlerItem(handler, handledEventsToo));
        }

        private static void RegisterClassHandler(Type classType, RoutedEvent routedEvent, RoutedEventHandlerItem eventRouteItem)
        {
            ClassHandlerKey key = new ClassHandlerKey(classType, routedEvent);

            if (flattenedClassHandlersCache.Contains(key))
            {
                throw new Granular.Exception("{0} class handler for \"{1}\" has already been queried, RegisterClassHandler should only be called from {0}'s static constructor", classType.Name, routedEvent);
            }

            classHandlers.Add(key, eventRouteItem);
        }

        public static IEnumerable<RoutedEventHandlerItem> GetClassHandlers(Type classType, RoutedEvent routedEvent)
        {
            return classHandlers.GetValues(new ClassHandlerKey(classType, routedEvent));
        }

        public static IEnumerable<RoutedEvent> GetRoutedEvents(Type containingType, bool flattenHierarchy)
        {
            Granular.Compatibility.RuntimeHelpers.RunClassConstructor(containingType);
            return registeredRoutedEvents.Keys.
                Where(key => key.Owner == containingType || flattenHierarchy && key.Owner.IsAssignableFrom(containingType)).
                Select(key => registeredRoutedEvents[key]);
        }

        public static IEnumerable<RoutedEvent> GetRoutedEvents(Type containingType, string eventName, bool flattenHierarchy)
        {
            Granular.Compatibility.RuntimeHelpers.RunClassConstructor(containingType);
            return registeredRoutedEvents.Keys.
                Where(key => key.Name == eventName && (key.Owner == containingType || flattenHierarchy && key.Owner.IsAssignableFrom(containingType))).
                Select(key => registeredRoutedEvents[key]);
        }

        public static RoutedEvent FindRoutedEvent(Type containingType, string eventName)
        {
            RoutedEvent[] routedEvents = GetRoutedEvents(containingType, eventName, true).ToArray();

            if (routedEvents.Length > 1)
            {
                throw new Granular.Exception("Type \"{0}\" contains more than one event named \"{1}\" ({2})", containingType.Name, eventName, routedEvents.Select(routedEvent => routedEvent.ToString()).Aggregate((s1, s2) => String.Format("{0}, {1}", s1, s2)));
            }

            return routedEvents.FirstOrDefault();
        }

        public static RoutedEvent GetOwnedRoutedEvent(Type ownerType, string eventName)
        {
            return GetRoutedEvents(ownerType, eventName, false).SingleOrDefault();
        }

        public static RoutedEvent GetRoutedEvent(XamlName eventName)
        {
            if (!eventName.IsMemberName)
            {
                throw new Granular.Exception("Invalid routed event name \"{0}\"", eventName.LocalName);
            }

            Type ownerType = TypeParser.ParseType(eventName.ContainingTypeName);
            return GetOwnedRoutedEvent(ownerType, eventName.MemberName);
        }

        public static IEnumerable<RoutedEventHandlerItem> GetFlattenedClassHandlers(Type classType, RoutedEvent routedEvent)
        {
            return flattenedClassHandlersCache.GetValue(new ClassHandlerKey(classType, routedEvent));
        }

        private static IEnumerable<RoutedEventHandlerItem> ResolveFlattenedClassHandlers(ClassHandlerKey key)
        {
            IEnumerable<RoutedEventHandlerItem> handlers = new RoutedEventHandlerItem[0];

            Type type = key.ClassType;
            while (type != null)
            {
                handlers = classHandlers.GetValues(new ClassHandlerKey(type, key.RoutedEvent)).Concat(handlers);
                type = type.BaseType;
            }

            return handlers.ToArray();
        }
    }
}
