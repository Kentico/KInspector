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
                Comment = @"Ensures that the 'CMSHashStringSalt' key is found within the web configuration file."
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
           if(ConfigurationManager.AppSettings["CMSHashStringSalt"] != null)
           {
                return new ModuleResults
                {
                    Status = Status.Good,
                    ResultComment = "CMSHashStringSalt key found!"
                };
           }
           else
           {
                return new ModuleResults
                {
                    Status = Status.Error,
                    ResultComment = "CMSHashStringSalt key not found!"
                };
           }
        }
    }
}
