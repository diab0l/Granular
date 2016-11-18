using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Granular.Extensions;
using Mono.Cecil;

namespace Granular.BuildTasks
{
    public class ReflectionTypeParser : ITypeParser
    {
        public class XmlnsDefinition
        {
            public string XmlNamespace { get; private set; }
            public string ClrNamespace { get; private set; }
            public string AssemblyName { get; private set; }

            public XmlnsDefinition(string xmlNamespace, string clrNamespace, string assemblyName)
            {
                this.XmlNamespace = xmlNamespace;
                this.ClrNamespace = clrNamespace;
                this.AssemblyName = assemblyName;
            }
        }

        private static readonly string ClrNamespacePrefix = "clr-namespace:";
        private static readonly string AssemblyQualifier = ";assembly=";
        private static readonly string XmlnsDefinitionAttributeTypeFullName = "System.Windows.Markup.XmlnsDefinitionAttribute";

        private IEnumerable<AssemblyDefinition> assemblyDefinitions;
        private IEnumerable<XmlnsDefinition> xmlnsDefinitions;

        public ReflectionTypeParser(IEnumerable<string> referenceAssemblies)
        {
            assemblyDefinitions = referenceAssemblies.Where(File.Exists).Select(AssemblyDefinition.ReadAssembly).ToArray();
            xmlnsDefinitions = assemblyDefinitions.SelectMany(GetXmlnsDefinitions).ToArray();
        }

        public bool TryParseTypeName(string localName, string namespaceName, out string typeName)
        {
            if (namespaceName.StartsWith(ClrNamespacePrefix))
            {
                string clrNamespace = GetClrNamespace(namespaceName.Substring(ClrNamespacePrefix.Length));
                typeName = String.Format("{0}.{1}", clrNamespace, localName);
                return true;
            }

            foreach (XmlnsDefinition xmlnsDefinition in xmlnsDefinitions)
            {
                if (xmlnsDefinition.XmlNamespace != namespaceName)
                {
                    continue;
                }

                AssemblyDefinition assemblyDefinition = assemblyDefinitions.FirstOrDefault(a => a.Name.Name == xmlnsDefinition.AssemblyName);

                if (assemblyDefinition == null)
                {
                    continue;
                }

                typeName = String.Format("{0}.{1}", xmlnsDefinition.ClrNamespace, localName);
                if (assemblyDefinition.MainModule.GetType(typeName) != null)
                {
                    return true;
                }
            }

            typeName = String.Empty;
            return false;
        }

        private static string GetClrNamespace(string qualifiedNamespace)
        {
            int assemblyQualifierIndex = qualifiedNamespace.IndexOf(AssemblyQualifier);
            return assemblyQualifierIndex == -1 ? qualifiedNamespace : qualifiedNamespace.Substring(0, assemblyQualifierIndex);
        }

        private static IEnumerable<XmlnsDefinition> GetXmlnsDefinitions(AssemblyDefinition assemblyDefinition)
        {
            foreach (CustomAttribute customAttribute in assemblyDefinition.CustomAttributes)
            {
                if (customAttribute.AttributeType.FullName != XmlnsDefinitionAttributeTypeFullName || customAttribute.ConstructorArguments.Count < 2)
                {
                    continue;
                }

                string xmlNamespace = (string)customAttribute.ConstructorArguments[0].Value;
                string clrNamespace = (string)customAttribute.ConstructorArguments[1].Value;
                string assemblyName = customAttribute.ConstructorArguments.Count > 2 ? (string)customAttribute.ConstructorArguments[2].Value : assemblyDefinition.Name.Name;

                yield return new XmlnsDefinition(xmlNamespace, clrNamespace, assemblyName);
            }
        }
    }
}
