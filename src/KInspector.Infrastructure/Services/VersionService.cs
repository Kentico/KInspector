using System.Diagnostics;

using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Infrastructure.Models.Data;

namespace KInspector.Infrastructure.Services
{
    public class VersionService : IVersionService
    {
        private readonly IDatabaseService databaseService;

        private static readonly string getCmsSettingsPath = @"Scripts/GetCmsSettings.sql";

        private const string _DBMajorVersionKeyName = "CMSDBVersion";
        private const string _DMHotfixVersionKeyName = "CMSHotfixVersion";

        private const string _administrationDllToCheck = "CMS.DataEngine.dll";
        private const string _relativeAdministrationDllPath = "bin";
        private const string _relativeHotfixFileFolderPath = "App_Data\\Install";
        private const string _hotfixFile = "Hotfix.txt";

        public VersionService(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public Version? GetKenticoAdministrationVersion(Instance instance)
        {
            return string.IsNullOrEmpty(instance.AdministrationPath) ? null : GetKenticoAdministrationVersion(instance.AdministrationPath);
        }

        public Version? GetKenticoAdministrationVersion(string rootPath)
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

        public Version? GetKenticoDatabaseVersion(DatabaseSettings databaseSettings)
        {
            databaseService.Configure(databaseSettings);

            var settingsKeys = databaseService.ExecuteSqlFromFile<CmsSettingsKey>(getCmsSettingsPath)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            var majorVersionString = settingsKeys.FirstOrDefault(key => key.KeyName == _DBMajorVersionKeyName)?.KeyValue;
            var hotfixVersionString = settingsKeys.FirstOrDefault(key => key.KeyName == _DMHotfixVersionKeyName)?.KeyValue;

            if (majorVersionString is null)
            {
                throw new InvalidDataException("Database Major version unknown. CMSDBVersion not found in table CMS_SettingsKey.");
            }

            var version = new Version(majorVersionString);
            if (version.Major > 13)
            {
                return version;
            }

            if (hotfixVersionString is null)
            {
                throw new InvalidDataException("Database Hotfix version unknown. CMSHotfixVersion not found in table CMS_SettingsKey.");
            }

            return new Version($"{majorVersionString}.{hotfixVersionString}");
        }
    }
}