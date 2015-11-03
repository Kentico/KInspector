using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class AttachmentsBySizeModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Attachments by size",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = 
@"Displays report of all attachments ordered by their size.

Storing files in a content tree can negatively affect website performance. If you're not using workflow or multilingual content features to manipulate those attachments, you can move them to the media library.

For more information, see the documentation:
https://docs.kentico.com/display/K82/Defining+website+data+structure#Definingwebsitedatastructure-Storingdataefficiently",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("AttachmentsBySizeModule.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
