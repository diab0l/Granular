using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Granular.Extensions;

namespace Granular.BuildTasks
{
    public class ReflectionTypeParser : MarshalByRefObject, ITypeParser
    {
        public class LocalXmlnsDefinitionAttribute
        {
            public string XmlNamespace { get; private set; }
            public string ClrNamespace { get; private set; }
            public string AssemblyName { get; private set; }

            public LocalXmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace, string assemblyName = null)
            {
                this.XmlNamespace = xmlNamespace;
                this.ClrNamespace = clrNamespace;
                this.AssemblyName = assemblyName;
            }
        }

        private static string ClrNamespacePrefix = "clr-namespace:";
        private static readonly string AssemblyQualifier = ";assembly=";

        private static readonly IEnumerable<LocalXmlnsDefinitionAttribute> XmlnsDefinitions = new[]
        {
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows", "Granular.Presentation"),
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls", "Granular.Presentation"),
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Primitives", "Granular.Presentation"),
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Data", "Granular.Presentation"),
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Input", "Granular.Presentation"),
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media", "Granular.Presentation"),
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Animation", "Granular.Presentation"),
            new LocalXmlnsDefinitionAttribute("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Imaging", "Granular.Presentation"),
        };

        private enum XamlItemType { XamlPage, XamlApplicationDefinition };

        private List<Type> allTypes;

        public ReflectionTypeParser()
        {
            allTypes = new List<Type>();
        }

        public void LoadAssemblies(IEnumerable<string> referenceAssemblies)
        {
            foreach (string referenceAssembly in referenceAssemblies)
            {
                if (File.Exists(referenceAssembly))
                {
                    Assembly assembly = Assembly.ReflectionOnlyLoadFrom(referenceAssembly);
                    allTypes.AddRange(GetAssemblyTypes(assembly).Where(type => type != null));
                }
            }
        }

        private IEnumerable<Type> GetAssemblyTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types;
            }
        }

        public bool TryParseTypeName(string localName, string namespaceName, out string typeName)
        {
            if (namespaceName.StartsWith(ClrNamespacePrefix))
            {
                string clrNamespace = GetClrNamespace(namespaceName.Substring(ClrNamespacePrefix.Length));
                typeName = String.Format("{0}.{1}", clrNamespace, localName);
                return true;
            }

            foreach (LocalXmlnsDefinitionAttribute xmlnsDefinition in XmlnsDefinitions)
            {
                if (xmlnsDefinition.XmlNamespace == namespaceName &&
                    TryFindTypeName(localName, xmlnsDefinition.ClrNamespace, xmlnsDefinition.AssemblyName, out typeName))
                {
                    return true;
                }
            }

            typeName = String.Empty;
            return false;
        }

        private bool TryFindTypeName(string localName, string clrNamespace, string assemblyName, out string typeName)
        {
            Type type = allTypes.FirstOrDefault(t => t.Name == localName && t.Namespace == clrNamespace && (assemblyName.IsNullOrEmpty() || t.Assembly.GetName().Name == assemblyName));

            if (type != null)
            {
                typeName = type.FullName;
                return true;
            }

            typeName = String.Empty;
            return false;
        }

        private static string GetClrNamespace(string qualifiedNamespace)
        {
            int assemblyQualifierIndex = qualifiedNamespace.IndexOf(AssemblyQualifier);
            return assemblyQualifierIndex == -1 ? qualifiedNamespace : qualifiedNamespace.Substring(0, assemblyQualifierIndex);
        }
    }
}
