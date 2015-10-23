using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class ApplicationRestartsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Application restarts",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Displays information about application restarts.

Frequent restarts could signify some troubles.",
                Category = "Event log"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("ApplicationRestartsModule.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
