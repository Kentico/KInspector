using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class CMSFileModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "CMSFile usage check (takes a while)",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Comment = @"Checks whethere there are unnecessary CMSFiles in the content tree",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetPrintsFromFile("CMSFileModule.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
