using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Granular.BuildTasks
{
    public class MemberDefinition
    {
        public string Name { get; private set; }
        public string TypeName { get; private set; }

        public MemberDefinition(string name, string typeName)
        {
            this.Name = name;
            this.TypeName = typeName;
        }

        public override bool Equals(object obj)
        {
            MemberDefinition other = obj as MemberDefinition;

            return other != null && this.GetType() == other.GetType() &&
                this.Name == other.Name && this.TypeName == other.TypeName;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ TypeName.GetHashCode();
        }
    }

    public class ClassDefinition
    {
        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public string BaseTypeName { get; private set; }
        public IEnumerable<MemberDefinition> Members { get; private set; }

        public ClassDefinition(string name, string @namespace, string baseTypeName, IEnumerable<MemberDefinition> members)
        {
            this.Name = name;
            this.Namespace = @namespace;
            this.BaseTypeName = baseTypeName;
            this.Members = members;
        }
    }

    public static class ClassDefinitionExtensions
    {
        public static CodeTypeDeclaration CreateClassDeclaration(this ClassDefinition classDefinition)
        {
            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration(classDefinition.Name)
            {
                IsClass = true,
                IsPartial = true,
                TypeAttributes = TypeAttributes.Public,
            };

            classDeclaration.BaseTypes.Add(new CodeTypeReference(classDefinition.BaseTypeName));

            foreach (MemberDefinition member in classDefinition.Members)
            {
                classDeclaration.Members.Add(new CodeMemberField
                {
                    Attributes = MemberAttributes.Assembly,
                    Type = new CodeTypeReference(member.TypeName),
                    Name = member.Name,
                });
            }

            return classDeclaration;
        }
    }
}
