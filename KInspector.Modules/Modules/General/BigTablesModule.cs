using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class BigTablesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Top 25 tables by size",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Displays top 25 biggest tables from the database.",
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;

            int databaseSizeInMB = dbService.ExecuteAndGetScalar<int>("SELECT SUM(reserved_page_count) * 8.0 / 1024 FROM sys.dm_db_partition_stats");
            var results = dbService.ExecuteAndGetTableFromFile("BigTablesModule.sql");

            return new ModuleResults
            {
                Result = results,
                ResultComment = String.Format("The overall database size is {0} MB", databaseSizeInMB)
            };
        }
    }
}
