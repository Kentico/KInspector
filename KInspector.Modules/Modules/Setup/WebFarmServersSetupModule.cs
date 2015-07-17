using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.Setup
{
    public class WebFarmServersSetupModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Disable enabled Web farm servers",
                Comment = "Disables Web farm servers defined in the Web farm application.\nThe servers are disabled by setting their Enabled state to disabled and their disaplay name is appended with '.disabled', so that servers disabled by the audit can be identified.",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Category = "Setup",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetDataSetFromFile("Setup/WebFarmServerSetupModule.sql");

            return new ModuleResults
            {
                Result = results
            };
        }
    }
}
