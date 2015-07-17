using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.Content
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
                    new Version("8.2") 
                },
                Comment = @"Selects all TreeNodes that have more than 1000 children",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("TreeNodeChildrenModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Structure the content in the content tree so that there is no element with 1000 or more children",
                    Status = Status.Error,
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
