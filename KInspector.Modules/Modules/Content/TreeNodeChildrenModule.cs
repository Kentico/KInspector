using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class TreeNodeChildrenModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Number of children of TreeNode",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Displays TreeNodes having more than 1000 children.

Our best practice is to store under each TreeNode maximum of 1000 children, otherwise it can negatively affect the performance.",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("TreeNodeChildrenModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Structure the content in the content tree so that there is no element with 1000 or more children.",
                    Status = Status.Warning,
                };
            }

            return new ModuleResults
            {
                Status = Status.Good,
                ResultComment = "There are no tree nodes with more than 1000 children, everything is OK.",
            };
        }
    }
}
