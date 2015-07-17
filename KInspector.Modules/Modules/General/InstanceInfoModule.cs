using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.General
{
    public class InstanceInfoModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Kentico instance information",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Comment = @"Shows various information about the Kentico instance.",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetDataSetFromFile("InstanceInfo.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
