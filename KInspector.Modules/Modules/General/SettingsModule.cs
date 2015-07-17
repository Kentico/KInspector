using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.General
{
    public class SettingsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Important settings",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Comment = @"Selects important settings from the database",
                
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("SettingsModule.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
