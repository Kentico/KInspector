using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class WebPartsInTemplatesAndTransformationsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Web parts in page templates and transformations",
                Comment = "Looks up page templates and transformations containing certain web parts.",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Category = "Content",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile("WebPartsInTemplatesAndTransformationsModule.sql");

            return new ModuleResults
            {
                Result = results
            };
        }
    }
}
