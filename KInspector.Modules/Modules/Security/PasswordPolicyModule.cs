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
                Name = "Password policy settings",
                Comment = @"It is critical that passwords are stored securely in the database, the module checks that the default and recommended SHA2 option is configured. https://docs.kentico.com/display/K9/Password+encryption+in+database
This module also checks that there is a password policy enforced to ensure users use a password that meets a minimum set of requirements. https://docs.kentico.com/display/K9/Password+strength+policy+and+its+enforcement",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Security",
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("PasswordPolicy.sql");

            if (results.Rows.Count > 0)
            {
                DataRow[] passwordFormatRows = results.Select("KeyName = 'CMSPasswordFormat'");
                DataRow[] passwordPolicyRows = results.Select("KeyName = 'CMSUsePasswordPolicy'");

                if (passwordFormatRows.Any(r => r["KeyValue"].ToString() != "SHA2SALT"))
                        {
                            return new ModuleResults
                            {
                                Result = results,
                                ResultComment = "The CMSPasswordFormat should be set to 'SHA2SALT'.",
                                Status = Status.Error,
                            };
                        } 
                else if(passwordPolicyRows.Any(r => r["KeyValue"].ToString() != "True"))
                            {
                                return new ModuleResults
                                {
                                    Result = results,
                                    ResultComment = "It is recommended that you have CMSUsePasswordPolicy set to 'True'.",
                                    Status = Status.Warning,
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