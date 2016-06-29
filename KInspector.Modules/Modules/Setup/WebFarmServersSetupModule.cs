using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class WebFarmServersSetupModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Disable enabled Web farm servers",
                Comment = @"Disables Web farm servers defined in the Web farm application.

The servers are disabled by setting their Enabled state to disabled and their display name is appended with '.disabled', so that servers disabled by the audit can be identified.",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Setup",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile("Setup/WebFarmServerSetupModule.sql");

            return new ModuleResults
            {
                Result = results
            };
        }
    }
}
