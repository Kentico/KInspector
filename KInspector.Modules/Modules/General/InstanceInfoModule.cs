using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
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
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Shows various information about the Kentico instance.",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile("InstanceInfo.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
