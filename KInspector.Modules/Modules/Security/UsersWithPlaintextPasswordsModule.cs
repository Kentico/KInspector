using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class UsersWithPlaintextPasswordsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Users with plaintext passwords",
                SupportedVersions = new[] {
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Security",
                Comment = @"Displays a list of all users with passwords that are stored in plain text.

Passwords stored in plain text greatly increase the risk to users in the event that unauthorized users gain access your database and increase the risk that your site may be accessed through a brute force attack.
By ensuring that all stored password are salted and hashed, you can significantly mitigate the damage done in the event of a database breach, and decrease the risk of unauthorized access to sensitive parts of your site.

For more information, see the documentation:
https://docs.kentico.com/display/K82/Password+encryption+in+database"

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
