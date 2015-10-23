using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class DatabaseConsistencyCheckModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Database consistency check",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Runs DBCC CHECKDB on current database which checks all consistency issues (https://msdn.microsoft.com/en-us/library/ms176064.aspx).",
                Category = "Consistency issues"
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("DatabaseConsistencyCheckModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    ResultComment = "CHECKDB found some errors!",
                    Result = results,
                    Status = Status.Error
                };
            }

            return new ModuleResults
            {
                Status = Status.Good,
                ResultComment = "CHECKDB didn't found any errors."
            };
        }
    }
}
