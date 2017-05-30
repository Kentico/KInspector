using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class UnusedPageTypesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Unused page types",
                SupportedVersions = new[] {
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0")
                },
                Comment = @"Looks for unused page types.",
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var unusedTemplates = dbService.ExecuteAndGetTableFromFile("UnusedPageTypesModule.sql");

            return new ModuleResults
            {
                Result = unusedTemplates
            };
        }
    }
}
