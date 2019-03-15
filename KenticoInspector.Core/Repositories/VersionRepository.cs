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
            var fileVersion = fileVersionInfo.FileVersion;
            return new Version(fileVersion);
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
                    var query = "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSDBVersion'";
                    connection.Open();
                    var version = connection.QuerySingle<string>(query);
                    return new Version(version);
                }
            }
            catch
            {
                return null;
            }
        }
    }

}
