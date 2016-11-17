using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections;
using System.Text.RegularExpressions;
using Granular.Extensions;

namespace Granular.BuildTasks
{
    public class WriteGeneratedItemsTask : Task
    {
        private const string CompileItemType = "Compile";
        private const string IsGeneratedItemMetadataName = "IsGeneratedItem";

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public ITaskItem[] CompileItems { get; set; }

        [Required]
        public string OutputTargetsPath { get; set; }

        public string ExcludeFilter { get; set; }

        public bool AttachDebugger { get; set; }

        public override bool Execute()
        {
            if (AttachDebugger)
            {
                System.Diagnostics.Debugger.Launch();
            }

            try
            {
                Regex excludeFilterRegex = !ExcludeFilter.IsNullOrEmpty() ? new Regex(ExcludeFilter) : null;

                using (ProjectCollection collection = new ProjectCollection())
                {
                    Project sourceProject = collection.LoadProject(ProjectPath);
                    Project targetProject = new Project(collection);

                    AddGeneratedItems(targetProject, GetGeneratedItems(CompileItems, sourceProject.GetItems(CompileItemType), excludeFilterRegex), CompileItemType);

                    targetProject.Save(OutputTargetsPath);
                }
            }
            catch (Exception e)
            {
                Log.LogError($"{nameof(WriteGeneratedItemsTask)} - Error reading \"{ProjectPath}\"");
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }

        private static void AddGeneratedItems(Project targetProject, IEnumerable<ITaskItem> items, string itemType)
        {
            foreach (ITaskItem item in items)
            {
                ProjectItem projectItem = targetProject.AddItem(itemType, item.ItemSpec).Single();
                projectItem.SetMetadataValue(IsGeneratedItemMetadataName, "true");
            }
        }

        private static IEnumerable<ITaskItem> GetGeneratedItems(ITaskItem[] items, IEnumerable<ProjectItem> projectItems, Regex excludeFilterRegex)
        {
            IEnumerable<ITaskItem> generatedItems = items.Where(item => !projectItems.Any(projectItem => projectItem.EvaluatedInclude == item.ItemSpec && projectItem.GetMetadataValue(IsGeneratedItemMetadataName) != "true"));

            if (excludeFilterRegex != null)
            {
                generatedItems = generatedItems.Where(item => !excludeFilterRegex.IsMatch(item.ItemSpec));
            }

            return generatedItems.ToArray();
        }
    }
}
