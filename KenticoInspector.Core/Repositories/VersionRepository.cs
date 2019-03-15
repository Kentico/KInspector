using Dapper;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IO;

namespace KenticoInspector.Core.Services
{
    public class VersionRepository : IVersionRepository
    {
        const string _administrationDllToCheck = "CMS.DataEngine.dll";
        const string _relativeAdministrationDllPath = "bin";
        const string _relativeHotfixFileFolderPath = "App_Data\\Install";
        const string _hotfixFile = "Hotfix.txt";

        public Version GetKenticoAdministrationVersion(Instance instance)
        {
            return GetKenticoAdministrationVersion(instance.Path);
        }

        public Version GetKenticoAdministrationVersion(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                return null;
            }

            var binDirectory = Path.Combine(rootPath, _relativeAdministrationDllPath);
            if (!Directory.Exists(binDirectory))
            {
                return null;
            }

            var dllFileToCheck = Path.Combine(binDirectory, _administrationDllToCheck);
            if (!File.Exists(dllFileToCheck))
            {
                return null;
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(dllFileToCheck);

            var hotfix = "0";
            var hotfixDirectory = Path.Combine(rootPath, _relativeHotfixFileFolderPath);
            if (Directory.Exists(hotfixDirectory))
            {
                var hotfixFile = Path.Combine(hotfixDirectory, _hotfixFile);
                if (File.Exists(hotfixFile))
                {
                    hotfix = File.ReadAllText(hotfixFile);
                }
            }

            var version = $"{fileVersionInfo.FileMajorPart}.{fileVersionInfo.FileMinorPart}.{hotfix}";
            return new Version(version);
        }
        
        public Version GetKenticoDatabaseVersion(Instance instance)
        {
            return GetKenticoDatabaseVersion(instance.DatabaseSettings);
        }

        public Version GetKenticoDatabaseVersion(DatabaseSettings databaseSettings)
        {
            try
            {
                var instanceConnection = DatabaseHelper.GetSqlConnection(databaseSettings);

                using (var connection = instanceConnection)
                {
                    connection.Open();
                    var version = connection.QuerySingle<string>("SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSDBVersion'");
                    var hotfix = connection.QuerySingle<string>("SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSHotfixVersion'");
                    return new Version($"{version}.{hotfix}");
                }
            }
            catch
            {
                return null;
            }
        }
    }

}
