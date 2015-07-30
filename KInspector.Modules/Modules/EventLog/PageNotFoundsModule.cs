using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.EventLog
{
    public class PageNotFoundsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Not found errors (404)",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Comment = @"Displays all the 404 errors from the event log.

If there are any 404 errors, maybe there is a broken link somewhere on your pages. 
Check the referrer and try to fix all the broken links. For the other links, try to fix a root cause.
In general, you want to avoid this error because it's an unnecessary waste of resources.",
                Category = "Event log"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("PageNotFoundsModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Page not founds found! Check the referrers, if the links to these non existing pages can be removed",
                    Status = results.Rows.Count > 10 ? Status.Error : Status.Warning,
                };
            }

            return new ModuleResults
            {
                ResultComment = "No page not founds were found in the event log.",
                Status = Status.Good
            };
        }
    }
}
