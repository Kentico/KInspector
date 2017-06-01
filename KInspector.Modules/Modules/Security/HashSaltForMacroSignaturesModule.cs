using System;
using Kentico.KInspector.Core;
using System.Configuration;

namespace Kentico.KInspector.Modules
{
    public class HashSaltForMacroSignaturesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Category = "Security",
                Name = "Hash Salt For Macro Signatures",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"The CMSHashStringSalt is used in numerous hash functions within the system.  
                            This modules will ensure that the 'CMSHashStringSalt' key is found within the web configuration file.
                            To learn more about this key and the importance of it within the system, please visit 
                            https://docs.kentico.com/display/K9/Working+with+macro+signatures#Workingwithmacrosignatures-Configuringthehashsaltformacrosignatures."
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {

            string pathToWebConfig = instanceInfo.Directory.ToString();
            var kenticoVersion = instanceInfo.Version;

            // Update web.config path with "CMS" folder for Kentico 8 and newer versions
            if ((kenticoVersion >= new Version("8.0")) && !(instanceInfo.Directory.ToString().EndsWith("\\CMS\\") || instanceInfo.Directory.ToString().EndsWith("\\CMS")))
            {
                pathToWebConfig += "\\CMS";
            }
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = pathToWebConfig + "\\web.config" };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);          

            //Determine if the CMSHashStringSalt value is null or empty
            if(string.IsNullOrEmpty(configuration.AppSettings.Settings["CMSHashStringSalt"].Value))
            {
                return new ModuleResults
                {
                    Status = Status.Error,
                    ResultComment = "The 'CMSHashStringSalt' key is either empty or missing from the AppSettings in the web configuration file."
                };
            } else
            {
                return new ModuleResults
                {
                    Status = Status.Good,
                    ResultComment = "The 'CMSHashStringSalt' key was found!"
                };
            }

        }
    }
}
