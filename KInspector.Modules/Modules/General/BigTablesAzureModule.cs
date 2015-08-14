using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.General
{
    public class BigTablesAzureModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Size of the tables (Azure)",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Comment = @"Selects top 25 biggest tables from the database (Azure)",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("BigTablesModuleAzure.sql");

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
