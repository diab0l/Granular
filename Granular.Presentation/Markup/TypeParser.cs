using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xaml;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows.Markup
{
    public static class TypeParser
    {
        private static string ClrNamespacePrefix = "clr-namespace:";
        private static readonly string AssemblyQualifier = ";assembly=";

        private static readonly CacheDictionary<XamlName, Type> resolvedTypesCache = new CacheDictionary<XamlName, Type>(TryResolveType);
        private static IEnumerable<XmlnsDefinitionAttribute> xmlnsDefinitionAttributesCache;

        public static Type ParseType(string prefixedTypeName, XamlNamespaces namespaces)
        {
            return ParseType(XamlName.FromPrefixedName(prefixedTypeName, namespaces));
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryParseType(string prefixedTypeName, XamlNamespaces namespaces, out Type type)
        {
            return TryParseType(XamlName.FromPrefixedName(prefixedTypeName, namespaces), out type);
        }

        public static Type ParseType(XamlName name)
        {
            Type type;

            if (!TryParseType(name, out type))
            {
                throw new Granular.Exception("Type \"{0}\" wasn't found", name);
            }

            return type;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryParseType(XamlName name, out Type type)
        {
            return resolvedTypesCache.TryGetValue(name, out type);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryResolveType(XamlName name, out Type type)
        {
            if (XamlTypes.TryParseXamlType(name, out type))
            {
                return true;
            }

            XamlName extensionName = new XamlName(String.Format("{0}Extension", name.LocalName), name.NamespaceName);

            return TryGetType(name, out type) || TryGetType(extensionName, out type);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        private static bool TryGetType(XamlName xamlName, out Type type)
        {
            if (xamlName.NamespaceName.StartsWith(ClrNamespacePrefix))
            {
                string clrNamespace = GetClrNamespace(xamlName.NamespaceName.Substring(ClrNamespacePrefix.Length));
                string assemblyName = GetAssemblyName(xamlName.NamespaceName.Substring(ClrNamespacePrefix.Length));

                if (TryGetType(xamlName.LocalName, clrNamespace, assemblyName, out type))
                {
                    return true;
                }

                return false;
            }

            foreach (XmlnsDefinitionAttribute xmlnsDefinition in GetXmlnsDefinitionAttributes())
            {
                if (xmlnsDefinition.XmlNamespace == xamlName.NamespaceName &&
                    TryGetType(xamlName.LocalName, xmlnsDefinition.ClrNamespace, xmlnsDefinition.AssemblyName, out type))
                {
                    return true;
                }
            }

            type = null;
            return false;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        private static bool TryGetType(string localName, string clrNamespace, string assemblyName, out Type type)
        {
            if (!assemblyName.IsNullOrEmpty())
            {
                return TryGetType(String.Format("{0}.{1}, {2}", clrNamespace, localName, assemblyName), out type); ;
            }

            if (TryGetType(String.Format("{0}.{1}", clrNamespace, localName), out type))
            {
                return true;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (TryGetType(String.Format("{0}.{1}, {2}", clrNamespace, localName, assembly.GetName().Name), out type))
                {
                    return true;
                }
            }

            type = null;
            return false;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        private static bool TryGetType(string qualifiedTypeName, out Type type)
        {
            type = Granular.Compatibility.Type.GetType(qualifiedTypeName);
            return type != null;
        }

        private static string GetClrNamespace(string qualifiedNamespace)
        {
            int assemblyQualifierIndex = qualifiedNamespace.IndexOf(AssemblyQualifier);
            return assemblyQualifierIndex == -1 ? qualifiedNamespace : qualifiedNamespace.Substring(0, assemblyQualifierIndex);
        }

        private static string GetAssemblyName(string qualifiedNamespace)
        {
            int assemblyQualifierIndex = qualifiedNamespace.IndexOf(AssemblyQualifier);
            return assemblyQualifierIndex == -1 ? String.Empty : qualifiedNamespace.Substring(assemblyQualifierIndex + AssemblyQualifier.Length);
        }

        private static IEnumerable<XmlnsDefinitionAttribute> GetXmlnsDefinitionAttributes()
        {
            if (xmlnsDefinitionAttributesCache == null)
            {
                xmlnsDefinitionAttributesCache = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetCustomAttributesCached<XmlnsDefinitionAttribute>()).ToArray();
            }

            return xmlnsDefinitionAttributesCache;
        }
    }

    public static class XamlElementExtensions
    {
        public static Type GetElementType(this XamlElement element)
        {
            return TypeParser.ParseType(element.Name);
        }
    }
}
