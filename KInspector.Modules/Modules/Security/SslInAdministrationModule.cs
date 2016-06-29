using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class SslInAdministrationModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "SSL used for Administrative Interface",
                Comment = "Checks if pages of the administration interface are automatically redirected to https.",
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
            var results = dbService.ExecuteAndGetTableFromFile("SslInAdministration.sql");

            if (results.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = "True",
                    ResultComment = "SSL is configured for the administrative interface.",
                    Status = Status.Good,
                };
            }

            return new ModuleResults
            {
                Result = "False",
                ResultComment = "Go to Settings > Security & Membership > Administration to configure.",
                Status = Status.Warning,
            };
        }
    }
}
