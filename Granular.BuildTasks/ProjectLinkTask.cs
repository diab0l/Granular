using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Granular.BuildTasks
{
    public class ProjectLinkTask : Task
    {
        private const string IsProjectLinkItemMetadataName = "IsProjectLinkItem";
        private const string LinkMetadataName = "Link";

        private const string CompileItemType = "Compile";
        private const string PageItemType = "Page";
        private const string ApplicationDefinitionItemType = "ApplicationDefinition";
        private const string ResourceItemType = "Resource";
        private const string EmbeddedResourceItemType = "EmbeddedResource";

        [Required]
        public string SourceProjectPath { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public ITaskItem[] CompileItems { get; set; }

        [Required]
        public ITaskItem[] PageItems { get; set; }

        [Required]
        public ITaskItem[] ApplicationDefinitionItems { get; set; }

        [Required]
        public ITaskItem[] ResourceItems { get; set; }

        [Required]
        public ITaskItem[] EmbeddedResourceItems { get; set; }

        [Output]
        public ITaskItem[] OutputCompileItems { get; private set; }

        [Output]
        public ITaskItem[] OutputPageItems { get; private set; }

        [Output]
        public ITaskItem[] OutputApplicationDefinitionItems { get; private set; }

        [Output]
        public ITaskItem[] OutputResourceItems { get; private set; }

        [Output]
        public ITaskItem[] OutputEmbeddedResourceItems { get; private set; }

        public string[] ExcludeItems { get; set; }

        public bool RewriteProject { get; set; }

        public bool AttachDebugger { get; set; }

        public override bool Execute()
        {
            if (AttachDebugger)
            {
                System.Diagnostics.Debugger.Launch();
            }

            try
            {
                using (ProjectCollection collection = new ProjectCollection())
                {
                    Project sourceProject = collection.LoadProject(SourceProjectPath);

                    string sourceProjectDirectory = Path.GetDirectoryName(SourceProjectPath);

                    IEnumerable<string> excludeItems = ExcludeItems ?? new string[0];

                    OutputCompileItems = GetOutputItems(CompileItems, sourceProject, sourceProjectDirectory, CompileItemType, excludeItems).ToArray();
                    OutputPageItems = GetOutputItems(PageItems, sourceProject, sourceProjectDirectory, PageItemType, excludeItems).ToArray();
                    OutputApplicationDefinitionItems = GetOutputItems(ApplicationDefinitionItems, sourceProject, sourceProjectDirectory, ApplicationDefinitionItemType, excludeItems).ToArray();
                    OutputResourceItems = GetOutputItems(ResourceItems, sourceProject, sourceProjectDirectory, ResourceItemType, excludeItems).ToArray();
                    OutputEmbeddedResourceItems = GetOutputItems(EmbeddedResourceItems, sourceProject, sourceProjectDirectory, EmbeddedResourceItemType, excludeItems).ToArray();

                    if (RewriteProject)
                    {
                        Project project = collection.LoadProject(ProjectPath);

                        SetProjectLinkItems(project, OutputCompileItems.Where(IsProjectLinkItem).ToArray(), CompileItemType);
                        SetProjectLinkItems(project, OutputPageItems.Where(IsProjectLinkItem).ToArray(), PageItemType);
                        SetProjectLinkItems(project, OutputApplicationDefinitionItems.Where(IsProjectLinkItem).ToArray(), ApplicationDefinitionItemType);
                        SetProjectLinkItems(project, OutputResourceItems.Where(IsProjectLinkItem).ToArray(), ResourceItemType);
                        SetProjectLinkItems(project, OutputEmbeddedResourceItems.Where(IsProjectLinkItem).ToArray(), EmbeddedResourceItemType);

                        project.Save();
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogError($"ProjectLink - Error linking to \"{SourceProjectPath}\"");
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }

        private static IEnumerable<ITaskItem> GetOutputItems(IEnumerable<ITaskItem> existingItems, Project sourceProject, string sourceProjectDirectory, string itemType, IEnumerable<string> excludeItems)
        {
            existingItems = existingItems.Where(item => !IsProjectLinkItem(item)).ToArray();
            excludeItems = excludeItems.Concat(existingItems.Select(item => item.ItemSpec)).ToArray();

            return existingItems.Concat(GetOutputItems(sourceProject, sourceProjectDirectory, itemType, excludeItems)).ToArray();
        }

        private static IEnumerable<ITaskItem> GetOutputItems(Project sourceProject, string sourceProjectDirectory, string itemType, IEnumerable<string> excludeItems)
        {
            return sourceProject.GetItems(itemType).Where(item => !IsExcluded(item.UnevaluatedInclude, excludeItems)).Select(item => CreateTaskItem(item, sourceProjectDirectory)).ToArray();
        }

        private static ITaskItem CreateTaskItem(ProjectItem projectItem, string directory)
        {
            TaskItem taskItem = new TaskItem(Path.Combine(directory, projectItem.UnevaluatedInclude));

            taskItem.SetMetadata(LinkMetadataName, projectItem.UnevaluatedInclude);
            taskItem.SetMetadata(IsProjectLinkItemMetadataName, "true");

            foreach (ProjectMetadata metadata in projectItem.Metadata)
            {
                taskItem.SetMetadata(metadata.Name, metadata.UnevaluatedValue);
            }

            return taskItem;
        }

        private static void SetProjectLinkItems(Project project, ITaskItem[] linkItems, string itemType)
        {
            project.RemoveItems(project.Items.Where(projectItem => projectItem.ItemType == itemType && IsProjectLinkItem(projectItem) && !linkItems.Any(linkItem => linkItem.ItemSpec == projectItem.UnevaluatedInclude)).ToArray());

            foreach (ITaskItem linkItem in linkItems)
            {
                ProjectItem projectItem = project.Items.FirstOrDefault(existingProjectItem => linkItem.ItemSpec == existingProjectItem.UnevaluatedInclude) ?? project.AddItem(itemType, linkItem.ItemSpec).Single();
                CopyMetadata(projectItem, linkItem);
            }
        }

        private static void CopyMetadata(ProjectItem targetItem, ITaskItem sourceItem)
        {
            foreach (ProjectMetadata projectMetadata in targetItem.Metadata.ToArray())
            {
                if (String.IsNullOrEmpty(sourceItem.GetMetadata(projectMetadata.Name)))
                {
                    targetItem.Metadata.Remove(projectMetadata);
                }
            }

            foreach (DictionaryEntry entry in sourceItem.CloneCustomMetadata())
            {
                targetItem.SetMetadataValue((string)entry.Key, (string)entry.Value);
            }
        }

        private static bool IsExcluded(string itemPath, IEnumerable<string> excludeItems)
        {
            return excludeItems.Any(excludeItem => excludeItem == itemPath || excludeItem.EndsWith(Path.DirectorySeparatorChar.ToString()) && itemPath.StartsWith(excludeItem));
        }

        private static bool IsProjectLinkItem(ITaskItem item)
        {
            return item.GetMetadata(IsProjectLinkItemMetadataName) == "true";
        }

        private static bool IsProjectLinkItem(ProjectItem item)
        {
            return item.GetMetadataValue(IsProjectLinkItemMetadataName) == "true";
        }
    }
}
