using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridge
{
    public enum TypeAccessibility
    {
        None,
        Public,
        NonPrivate,
        Anonymous,
        NonAnonymous,
        All,
    }

    public enum MemberAccessibility
    {
        None,
        Public,
        Private,
        Protected,
        PublicAndProtected,
        Internal,
        NonPrivate,
        Constructor,
        PublicConstructor,
        PrivateConstructor,
        ProtectedConstructor,
        InternalConstructor,
        Event,
        PublicEvent,
        PrivateEvent,
        ProtectedEvent,
        InternalEvent,
        Field,
        PublicField,
        PrivateField,
        ProtectedField,
        InternalField,
        Method,
        PublicMethod,
        PrivateMethod,
        ProtectedMethod,
        InternalMethod,
        Property,
        PublicProperty,
        PrivateProperty,
        ProtectedProperty,
        InternalProperty,
        Instance,
        InstanceConstructor,
        PublicInstanceConstructor,
        PrivateInstanceConstructor,
        ProtectedInstanceConstructor,
        InternalInstanceConstructor,
        InstanceEvent,
        PublicInstanceEvent,
        PrivateInstanceEvent,
        ProtectedInstanceEvent,
        InternalInstanceEvent,
        InstanceField,
        PublicInstanceField,
        PrivateInstanceField,
        ProtectedInstanceField,
        InternalInstanceField,
        InstanceMethod,
        PublicInstanceMethod,
        PrivateInstanceMethod,
        ProtectedInstanceMethod,
        InternalInstanceMethod,
        InstanceProperty,
        PublicInstanceProperty,
        PrivateInstanceProperty,
        ProtectedInstanceProperty,
        InternalInstanceProperty,
        Static,
        StaticConstructor,
        PublicStaticConstructor,
        PrivateStaticConstructor,
        ProtectedStaticConstructor,
        InternalStaticConstructor,
        StaticEvent,
        PublicStaticEvent,
        PrivateStaticEvent,
        ProtectedStaticEvent,
        InternalStaticEvent,
        StaticField,
        PublicStaticField,
        PrivateStaticField,
        ProtectedStaticField,
        InternalStaticField,
        StaticMethod,
        PublicStaticMethod,
        PrivateStaticMethod,
        ProtectedStaticMethod,
        InternalStaticMethod,
        StaticProperty,
        PublicStaticProperty,
        PrivateStaticProperty,
        ProtectedStaticProperty,
        InternalStaticProperty,
        All,
    }

    public sealed class ReflectableAttribute : Attribute
    {
        public ReflectableAttribute()
        {
            //
        }

        public ReflectableAttribute(TypeAccessibility typeAccessibility)
        {
            //
        }

        public ReflectableAttribute(params MemberAccessibility[] memberAccessibilities)
        {
            //
        }

        public ReflectableAttribute(bool reflectable)
        {
            //
        }
    }
}
