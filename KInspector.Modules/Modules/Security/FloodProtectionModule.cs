using Kentico.KInspector.Core;
using System;

namespace Kentico.KInspector.Modules
{
    public class FloodProtectionModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Flood Protection",
                Comment = @"This module checks to see if Flood Protection is enabled on the site. This module also checks to make sure Flood Protection is enabled for the Chat module in V8 and later. https://docs.kentico.com/display/K9/Flood+protection",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Security",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            try
            {
                var dbService = instanceInfo.DBService;
                var results = dbService.ExecuteAndGetTableFromFile("FloodProtectionModule.sql");

                // Make sure there are records
                if (results.Rows.Count > 0)
                {
                    // Return the issues
                    return new ModuleResults
                    {
                        Result = results,
                        Status = Status.Warning
                    };
                }
                else
                {
                    return new ModuleResults
                    {
                        ResultComment = "Flood Protection is enabled.",
                        Status = Status.Good
                    };
                }
            }
            catch (Exception ex)
            {
                return new ModuleResults
                {
                    ResultComment = "Failed to check settings as expected.<br />" + ex.Message,
                    Status = Status.Error
                };
            }

        }
    }
}