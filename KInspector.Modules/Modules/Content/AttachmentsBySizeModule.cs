using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.Content
{
    public class AttachmentsBySizeModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Attachments by size",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"),
                    new Version("8.2") 
                },
                Comment = @"Shows report of all attachments ordered by their size.",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            var results = dbService.ExecuteAndGetTableFromFile("AttachmentsBySizeModule.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
