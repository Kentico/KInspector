using System;
using System.Data;
using System.Linq;
using System.IO;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class DisabledWebPartAnalyzerModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Disabled web parts and web part zones",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0")
                },
                Category = "General",
                Comment = @"Displays all page templates with disabled web parts and web part zones, meaning templates which have property 'visible' set to 'false'."
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("DisabledWebPartAnalyzerModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Page templates with disabled web parts found, check the table for the template names.",
                    Status = Status.Warning
                };
            }
            else
            {
                return new ModuleResults
                {
                    ResultComment = "No page templates with disabled web parts found.",
                    Status = Status.Good
                };
            }
        }
    }
}
