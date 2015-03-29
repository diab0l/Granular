using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;
using Microsoft.Build.Framework;

namespace Granular.BuildTasks
{
    public static class TaskItemExtensions
    {
        public static string GetRelativePath(this ITaskItem taskItem)
        {
            return taskItem.GetMetadata("Link").DefaultIfNullOrEmpty(taskItem.GetMetadata("Identity"));
        }

        public static IDictionary<string, string> GetMetadata(this ITaskItem taskItem)
        {
            return taskItem.MetadataNames.Cast<string>().ToDictionary(metadataName => metadataName, metadataName => taskItem.GetMetadata(metadataName));
        }
    }
}
