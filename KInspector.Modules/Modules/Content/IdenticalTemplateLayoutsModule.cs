using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class IdenticalTemplateLayoutsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Templates with identical layouts",
                SupportedVersions = new[] {
                    new Version("10.0")
                },
                Comment = @"Returns the list of templates with identical custom layouts (whitespace sensitive).",
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var identicalTemplateLayouts = dbService.ExecuteAndGetTableFromFile("IdenticalTemplateLayoutsModule.sql");

            return new ModuleResults
            {
                Result = identicalTemplateLayouts
            };
        }
    }
}
