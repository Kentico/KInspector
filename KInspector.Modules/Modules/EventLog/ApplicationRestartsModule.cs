using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.EventLog
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
                    new Version("8.2") 
                },
                Comment = @"Information on application restarts",
                Category = "Event log"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("ApplicationRestartsModule.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
