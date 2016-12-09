using Kentico.KInspector.Core;
using System;
using System.Web.Configuration;

namespace Kentico.KInspector.Modules.Modules.General
{
    class DebugCheckModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Category = "General",
                Name = "Debug Check",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Ensures that all debug keys in the CMS_SettingsKey table and the web.config file are set to false."
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("DebugCheckModule.sql");
            CompilationSection compilationSection = (CompilationSection)System.Configuration.ConfigurationManager.GetSection(@"system.web/compilation");

            if ((results.Rows.Count > 0 && ((int)results.Rows[0]["DebugCount"]) > 0) || compilationSection.Debug)
            {
                return new ModuleResults
                {
                    Status = Status.Error,
                    ResultComment = "Debug settings should be disabled on production instances!"
                };
            }
            else
            {
                return new ModuleResults
                {
                    Status = Status.Good,
                    ResultComment = "Debug settings have been disabled!"
                };
            }
        }
    }
}
