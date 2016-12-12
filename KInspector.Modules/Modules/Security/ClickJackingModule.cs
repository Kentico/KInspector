using System;
using System.Configuration;
using System.Linq;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class ClickJackingModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "ClickJacking Protection",
                Comment =
                    @"This module checks to see if protection against click jacking attacks is disabled for any paths. If the appSettings section of web.config file contains an etry CMSXFrameOptionsExclude, that means that the protection is disabled for the paths specified in the result . https://docs.kentico.com/display/K9/Clickjacking",
                SupportedVersions = new[]
                {
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
            ModuleResults result;

            Version kenticoVersion = instanceInfo.Version;
            string pathToWebConfig = instanceInfo.Directory.ToString();

            if ((kenticoVersion >= new Version("8.0")) && !(instanceInfo.Directory.ToString().EndsWith("\\CMS\\") || instanceInfo.Directory.ToString().EndsWith("\\CMS")))
            {
                pathToWebConfig += "\\CMS";
            }

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap { ExeConfigFilename = pathToWebConfig + "\\web.config" };
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            var entry = configuration.AppSettings.Settings["CMSXFrameOptionsExcluded"];
            bool hasEntry = entry != null;

            if (hasEntry)
            {
                result = new ModuleResults();
                result.Result = entry.Value;
                result.Status = Status.Warning;
                result.ResultComment =
                    @"Click jacking protection is disabled for the paths specified in ModuleResults.Result. See https://docs.kentico.com/display/K9/Clickjacking";
            }
            else
            {
                result = new ModuleResults();
                result.Status = Status.Good;
                result.ResultComment =
                    @"Click jacking protection is enabled by default. See https://docs.kentico.com/display/K9/Clickjacking";
            }

            return result;
        }
    }
}
