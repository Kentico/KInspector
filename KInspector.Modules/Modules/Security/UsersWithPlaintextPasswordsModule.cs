using System;
using KInspector.Core;

namespace KInspector.Modules.Modules.Security
{
    public class UsersWithPlaintextPasswordsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Users with plaintext passwords",
                Comment = "Selects all users with plain text passwords",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2") 
                },
                Category = "Security",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("UsersWithPlaintextPasswordsModule.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Users with plaintext passwords found, check the table for their names.",
                    Status = Status.Error,
                };
            }

            return new ModuleResults
            {
                ResultComment = "There are no users with plaintext passwords",
                Status = Status.Good
            };
        }
    }
}
