using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xaml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Granular.Extensions;

namespace Granular.BuildTasks
{
    public class CompileXaml : Task
    {
        private enum XamlItemType { XamlPage, XamlApplicationDefinition };

        [Required]
        public string TargetName { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string LanguageSourceExtension { get; set; }

        [Required]
        public string OutputPath { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        [Required]
        public ITaskItem[] XamlApplications { get; set; }

        [Required]
        public ITaskItem[] XamlPages { get; set; }

        [Required]
        public ITaskItem[] ReferenceAssemblies { get; set; }

        public bool AttachDebugger { get; set; }

        [Output]
        public ITaskItem[] GeneratedCodeFiles { get; private set; }

        public override bool Execute()
        {
            if (AttachDebugger)
            {
                System.Diagnostics.Debugger.Launch();
            }

            if (XamlApplications.Length > 1)
            {
                Log.LogError("More than one file has a GranularApplicationDefinition build action");

                foreach (ITaskItem xamlApplication in XamlApplications)
                {
                    Log.LogWarning(String.Empty, String.Empty, String.Empty, xamlApplication.GetRelativePath(), 0, 0, 0, 0, "This file has a GranularApplicationDefinition build action");
                }

                GeneratedCodeFiles = new ITaskItem[0];
                return false;
            }

            List<ITaskItem> generatedCodeFiles = new List<ITaskItem>();

            ReflectionTypeParser reflectionTypeParser = new ReflectionTypeParser(ReferenceAssemblies.Select(referenceAssembly => referenceAssembly.GetMetadata("FullPath")).ToArray());

            ITypeParser typeParser = new MarkupExtensionTypeParser(reflectionTypeParser);

            if (XamlApplications.Length == 1)
            {
                try
                {
                    ITaskItem generatedCodeFile = GenerateCodeFile(XamlApplications[0], typeParser, XamlItemType.XamlApplicationDefinition);
                    if (generatedCodeFile != null)
                    {
                        generatedCodeFiles.Add(generatedCodeFile);
                    }
                }
                catch (Exception e)
                {
                    Log.LogError(String.Empty, String.Empty, String.Empty, XamlApplications[0].GetRelativePath(), 0, 0, 0, 0, e.Message);
                    return false;
                }
            }

            foreach (ITaskItem xamlPage in XamlPages)
            {
                try
                {
                    ITaskItem generatedCodeFile = GenerateCodeFile(xamlPage, typeParser, XamlItemType.XamlPage);
                    if (generatedCodeFile != null)
                    {
                        generatedCodeFiles.Add(generatedCodeFile);
                    }
                }
                catch (Exception e)
                {
                    Log.LogError(String.Empty, String.Empty, String.Empty, xamlPage.GetRelativePath(), 0, 0, 0, 0, e.Message);
                    return false;
                }
            }

            GeneratedCodeFiles = generatedCodeFiles.ToArray();
            return true;
        }

        private ITaskItem GenerateCodeFile(ITaskItem item, ITypeParser typeParser, XamlItemType itemType)
        {
            XamlElement xamlElement = XamlParser.Parse(File.ReadAllText(item.GetMetadata("FullPath")));

            ClassDefinition classDefinition = XamlClassParser.Parse(xamlElement, typeParser);

            if (classDefinition == null)
            {
                return null;
            }

            CodeTypeDeclaration classDeclaration = classDefinition.CreateClassDeclaration();

            string itemRelativePath = item.GetRelativePath().Replace("\\", "/");
            string resourceUri = String.Format("/{0};component/{1}", TargetName, itemRelativePath);

            classDeclaration.Members.Add(CreateInitializeComponentMethod(resourceUri));

            if (itemType == XamlItemType.XamlApplicationDefinition)
            {
                classDeclaration.Members.Add(CreateEntryPointMethod(classDefinition.Name));
            }

            CodeNamespace classNamespace = new CodeNamespace(classDefinition.Namespace);
            classNamespace.Types.Add(classDeclaration);

            CodeCompileUnit classCompileUnit = new CodeCompileUnit();
            classCompileUnit.Namespaces.Add(classNamespace);

            string targetDirectory = Path.GetDirectoryName(Path.Combine(IntermediateOutputPath, item.GetMetadata("Link").DefaultIfNullOrEmpty(item.GetMetadata("Identity"))));
            Directory.CreateDirectory(targetDirectory);

            string targetFileName = String.Format("{0}.g{1}{2}", item.GetMetadata("Filename"), item.GetMetadata("Extension"), LanguageSourceExtension);

            return GenerateCodeFile(classCompileUnit, Path.Combine(targetDirectory, targetFileName));
        }

        private ITaskItem GenerateCodeFile(CodeCompileUnit compileUnit, string fullPath)
        {
            using (TextWriter writer = File.CreateText(fullPath))
            {
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CodeGeneratorOptions options = new CodeGeneratorOptions { BracingStyle = "C" };
                provider.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit("#pragma warning disable 0649"), writer, options);
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
                provider.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit("#pragma warning restore 0649"), writer, options);
            }

            return new TaskItem(fullPath);
        }

        private static CodeMemberMethod CreateInitializeComponentMethod(string resourceUri)
        {
            CodeMemberMethod initializeComponentMethod = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Private,
                Name = "InitializeComponent",
            };

            initializeComponentMethod.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("System.Windows.Application"), "LoadComponent", new CodeThisReferenceExpression(), new CodePrimitiveExpression(resourceUri))));

            return initializeComponentMethod;
        }

        private static CodeMemberMethod CreateEntryPointMethod(string className)
        {
            CodeMemberMethod entryPointMethod = new CodeEntryPointMethod();

            entryPointMethod.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(STAThreadAttribute))));

            entryPointMethod.Statements.Add(new CodeSnippetStatement("            System.Windows.ApplicationHost.Current.Run(() =>"));
            entryPointMethod.Statements.Add(new CodeSnippetStatement("            {"));
            entryPointMethod.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(className), "application", new CodeObjectCreateExpression(new CodeTypeReference(className))));
            entryPointMethod.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("application"), "InitializeComponent")));
            entryPointMethod.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("application"), "Run")));
            entryPointMethod.Statements.Add(new CodeSnippetStatement("            });"));

            return entryPointMethod;
        }
    }
}
