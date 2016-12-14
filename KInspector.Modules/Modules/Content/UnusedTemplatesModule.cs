using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class UnusedTemplatesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Unused templates",
                SupportedVersions = new[] {
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0")
                },
                Comment = @"Looks for unused templates.",
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var unusedTemplates = dbService.ExecuteAndGetTableFromFile("UnusedTemplatesModule.sql");

            return new ModuleResults
            {
                Result = unusedTemplates
            };
        }
    }
}
