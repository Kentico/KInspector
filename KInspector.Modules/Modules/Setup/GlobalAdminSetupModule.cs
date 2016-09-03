using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class GlobalAdminSetupModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Reset Global administrator",
                Comment = @"Enables and clears password of administrator user account if any.",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Setup",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile("Setup/GlobalAdminSetupModule.sql");

            return new ModuleResults
            {
                ResultComment = "The default administrator user with UserID=53 has been reset. You may need to recycle the application pool for changes to take effect."
            };
        }
    }
}
