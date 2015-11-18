using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class PageTypeAssignedToSiteModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Page type is assigned to a site",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Displays page types used by a certain site without being assigned to this site.

If the page type is not assigned to the site and is used by this site, you will encounter problems while using import/export feature.",
                Category = "Content"
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("PageTypeAssignedToSiteModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Issues found! You can fix this issue by assigning the page type to the site.",
                    Status = Status.Error,
                };
            }

            return new ModuleResults
            {
                ResultComment = "No issues found.",
                Status = Status.Good,
            };
        }
    }
}
