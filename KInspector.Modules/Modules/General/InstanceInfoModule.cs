using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class InstanceInfoModule : IModule
    {
        private static Dictionary<Version, string> VersionConfig => new Dictionary<Version, string>
        {
            [new Version("8.0")] = "InstanceInfo8.sql",
            [new Version("8.0")] = "InstanceInfo8.sql",
            [new Version("8.1")] = "InstanceInfo8.sql",
            [new Version("8.2")] = "InstanceInfo8.sql",
            [new Version("9.0")] = "InstanceInfo8.sql",
            [new Version("10.0")] = "InstanceInfo10.sql",
            [new Version("11.0")] = "InstanceInfo10.sql",
            [new Version("12.0")] = "InstanceInfo10.sql",
        };

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Kentico instance information",
                SupportedVersions = VersionConfig.Keys,
                Comment = @"Shows various information about the Kentico instance.",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile(VersionConfig[instanceInfo.Version]);

            return new ModuleResults
            {
                Result = results,
            };
        }
    }
}
