using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
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
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Displays all the 404 errors from the event log.

If there are any 404 errors, there is a broken link somewhere on your website. Check the referrer URL and fix all the broken links.

You should avoid broken links on your website as it hurts SEO and generates unnecessary traffic.

For the complete website analysis, you can use an external tool like www.deadlinkchecker.com.",
                Category = "Event log"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("PageNotFoundsModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Page not founds found! Check the referrers, if the links to these non existing pages can be removed.",
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
