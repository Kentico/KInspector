using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Configuration;
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
                Comment = "Checks that there is a password policy and that passwords are stored hashed and salted.",
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


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("PasswordPolicy.sql");

            DataRow[] passwordFormatRow;
            DataRow[] passwordPolicyRow;

            if (results.Rows.Count > 0)
            {
                passwordFormatRow = results.Select("KeyName = 'CMSPasswordFormat'");
                passwordPolicyRow = results.Select("KeyName = 'CMSUsePasswordPolicy'");

                // Return result depending on findings
                if (passwordFormatRow[0][1].ToString() != "SHA2SALT")
                {
                    return new ModuleResults
                    {
                        Result = results,
                        ResultComment = "The CMSPasswordFormat should be set to 'SHA2SALT'.",
                        Status = Status.Error,
                    };
                }
                else if (passwordPolicyRow[0][1].ToString() != "True")
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
