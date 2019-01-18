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
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0"),
                    new Version("12.0"), 
                },
                Category = "Setup",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;

            var sqlFile = instanceInfo.Version.Major <= 10
                ? "Setup/GlobalAdminSetupModule.sql"
                : "Setup/GlobalAdminSetupModule-V10.sql";

            var results = dbService.ExecuteAndGetDataSetFromFile(sqlFile);

            string pathToWebConfig = instanceInfo.Directory.ToString();

            if ((instanceInfo.Version.Major >= 8) &&
                !(instanceInfo.Directory.ToString().EndsWith("\\CMS\\") ||
                  instanceInfo.Directory.ToString().EndsWith("\\CMS")))
            {
                pathToWebConfig += "\\CMS";
            }

            pathToWebConfig += "\\web.config";

            var result = new ModuleResults();
            
            // Try touching the web.config to restart the site
            if (System.IO.File.Exists(pathToWebConfig))
            {
                System.IO.File.SetLastWriteTimeUtc(pathToWebConfig, DateTime.UtcNow);
                result.ResultComment =
                    "The default administrator user with UserID=53 has been reset. The application was restarted.";
            }
            else
            {
                result.ResultComment =
                    "The default administrator user with UserID=53 has been reset. Application could not be restarted. You may need to recycle the application pool for changes to take effect.";
            }

            return result;
        }
    }
}
