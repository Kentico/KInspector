using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.General
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
                    new Version("8.2") 
                },
                Comment = @"Runs DBCC CHECKDB on current database which checks all consistency issues.",
                Category = "Consistency issues"
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            try
            {
                var results = dbService.ExecuteAndGetTableFromFile("DatabaseConsistencyCheckModule.sql");

                return new ModuleResults
                {
                    Result = results,
                    Status =  Status.Error
                };
            }
            //TODO: temporary fix for dbService api
            // If no error is found, than no table is returned and dbService throws exception.
            catch (IndexOutOfRangeException)
            {
                return new ModuleResults
                {
                    Status = Status.Good,
                    ResultComment = "CHECKDB didn't found any errors."
                };
            }
        }
    }
}
