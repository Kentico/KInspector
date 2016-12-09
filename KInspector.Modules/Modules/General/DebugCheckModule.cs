using Kentico.KInspector.Core;
using System;
using System.Web.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

namespace Kentico.KInspector.Modules
{
    public class DebugCheckModule : IModule
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
            var disableAllDatabaseDebugs = false;
            var enabledDatabaseDebugs = new List<string>();

            bool compilationDebugActive = IsCompilationDebugActive(instanceInfo);

            var databaseDebugSettings = GetDatabaseDebugSettings(instanceInfo.DBService);
            foreach (var setting in databaseDebugSettings)
            {
                if (setting.Key == "CMSDisableDebug")
                {
                    disableAllDatabaseDebugs = setting.Value;
                }
                else if (setting.Value)
                {
                    enabledDatabaseDebugs.Add(string.Format("The {0} setting is enabled", setting.Key));
                }
            }

            var databaseDebugsActive = !disableAllDatabaseDebugs && enabledDatabaseDebugs.Count > 0;

            if (compilationDebugActive || databaseDebugsActive)
            {
                var result = new List<string>();

                if (compilationDebugActive)
                {
                    result.Add("Compilation debug is enabled in the web.config");
                }

                if (databaseDebugsActive)
                {
                    result.AddRange(enabledDatabaseDebugs);
                }

                return new ModuleResults
                {
                    Status = Status.Error,
                    Result = result,
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

        private static bool IsCompilationDebugActive(IInstanceInfo instanceInfo)
        {
            Version kenticoVersion = instanceInfo.Version;
            string pathToWebConfig = instanceInfo.Directory.ToString();

            if ((kenticoVersion >= new Version("8.0")) && !(instanceInfo.Directory.ToString().EndsWith("\\CMS\\") || instanceInfo.Directory.ToString().EndsWith("\\CMS")))
            {
                pathToWebConfig += "\\CMS";
            }

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap { ExeConfigFilename = pathToWebConfig + "\\web.config" };
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            CompilationSection compilationSection = (CompilationSection)configuration.GetSection(@"system.web/compilation");
            var compilationDebugActive = compilationSection != null ? compilationSection.Debug : false;
            return compilationDebugActive;
        }

        private Dictionary<string, bool> GetDatabaseDebugSettings(IDatabaseService databaseService)
        {
            var debugSettings = new Dictionary<string, bool>();

            var results = databaseService.ExecuteAndGetTableFromFile("DebugCheckModule.sql");
            foreach (DataRow debugSetting in results.Rows)
            {
                var key = debugSetting["Key"].ToString();
                var value = Boolean.Parse(debugSetting["Value"].ToString());

                debugSettings.Add(key, value);
            }

            return debugSettings;
        }
    }
}
