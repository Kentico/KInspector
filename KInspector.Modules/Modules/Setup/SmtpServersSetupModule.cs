using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class SmtpServersSetupModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Disable enabled SMTP servers",
                Comment = @"Disables SMTP servers defined either in the Settings or SMTP servers application.
                
The servers are disabled by setting their server address to invalid value, so that servers disabled by the audit can be identified.",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Setup",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile("Setup/SmtpServerSetupModule.sql");

            return new ModuleResults
            {
                Result = results
            };
        }
    }
}
