using System;
using System.Data;
using System.Linq;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class PasswordPolicyModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Password format settings",
                Comment = @"It is critical that passwords are stored securely in the database, the module checks that the recommended format is specified. https://docs.kentico.com/k12/securing-websites/designing-secure-websites/securing-user-accounts-and-passwords/setting-the-user-password-format",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0")
                },
                Category = "Security",
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("PasswordFormat.sql");

            if (results.Rows.Count > 0)
            {
                DataRow[] passwordFormatRows = results.Select("KeyName = 'CMSPasswordFormat'");

                if (instanceInfo.Version.Major < 10 &&  passwordFormatRows.Any(r => r["KeyValue"].ToString() != "SHA2SALT"))
                        {
                            return new ModuleResults
                            {
                                Result = results,
                                ResultComment = "The CMSPasswordFormat should be set to 'SHA2SALT'.",
                                Status = Status.Error,
                            };
                        } 
                else if (instanceInfo.Version.Major >= 10 && passwordFormatRows.Any(r => r["KeyValue"].ToString() != "PBKDF2"))
                {
                    return new ModuleResults
                    {
                        Result = results,
                        ResultComment = "The CMSPasswordFormat should be set to 'PBKDF2'.",
                        Status = Status.Error,
                    };
                }
                else 
                {
                    return new ModuleResults
                    {
                        ResultComment = "Password settings look good.",
                        Status = Status.Good
                    };
                }
            }

            return new ModuleResults
            {
                ResultComment = "Failed to check settings as expected.",
                Status = Status.Error
            };

        }
    }
}