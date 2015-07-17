using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.Content
{
    public class WebPartColumnsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Unspecified 'columns' setting in WebParts",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Comment = @"Checks WebParts where 'Columns' setting is missing",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            if (instanceInfo.Version == new Version("6.0"))
            {
                return new ModuleResults
                {
                    Result = dbService.ExecuteAndGetPrintsFromFile("WebPartColumnsModule6.sql"),
                };
            }
            return new ModuleResults
            {
                Result = dbService.ExecuteAndGetPrintsFromFile("WebPartColumnsModule.sql"),
            };
        }
    }
}
