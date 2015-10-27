using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class LicenseSetupModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Add license key for 'localhost' domain NOTE: Edit the 'LicenseSetupModule.sql' first.",
                Comment = "Adds license key for 'localhost' domain, removes existing 'localhost' license key (if any).",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Setup",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile("Setup/LicenseSetupModule.sql");

            return new ModuleResults
            {
                Result = results
            };
        }
    }
}
