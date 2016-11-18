using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;

namespace System.Windows
{
    public interface IResourceKey
    {
        Assembly Assembly { get; }
    }

    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public sealed class ComponentResourceKey : IResourceKey, IMarkupExtension
    {
        public Assembly Assembly { get { return TypeInTargetAssembly != null ? TypeInTargetAssembly.Assembly : null; } }

        public Type TypeInTargetAssembly { get; set; }

        public object ResourceId { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return this;
        }

        public override bool Equals(object obj)
        {
            ComponentResourceKey other = obj as ComponentResourceKey;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                Object.Equals(this.TypeInTargetAssembly, other.TypeInTargetAssembly) &&
                Object.Equals(this.ResourceId, other.ResourceId);
        }

        public override int GetHashCode()
        {
            return TypeInTargetAssembly.GetHashCode() ^ ResourceId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("ComponentResourceKey({0}, {1})", TypeInTargetAssembly.Name, ResourceId);
        }
    }
}
