using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Reports.DebugConfigurationAnalysis
{
    public class Report : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;
        readonly ICmsFileService _cmsFileService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService, ICmsFileService cmsFileService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
            _cmsFileService = cmsFileService;
        }

        public string Codename => "debug-configuration-analysis";

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0"),
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"";

        public string Name => "Debug Configuration Analysis";

        public string ShortDescription => "Shows status of debug settings in the database and web.config file.";

        public IList<string> Tags => new List<string> {
           ReportTags.Health
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

            var databaseSettingsValues = _databaseService.ExecuteSqlFromFile<SettingsKey>(Scripts.GetDebugSettingsValues);
            ResolveSettingsDisplayNames(instance, databaseSettingsValues);

            // TODO: Check web.config values (compilation debug & trace)

            return CompileResults(databaseSettingsValues);
        }

        private void ResolveSettingsDisplayNames(Instance instance, IEnumerable<SettingsKey> databaseSettingsValues)
        {
            var resxValues = _cmsFileService.GetResourceStringsFromResx(instance.Path);

            foreach (var databaseSettingsValue in databaseSettingsValues)
            {
                var key = databaseSettingsValue.KeyDisplayName
                    .Replace("{$", string.Empty)
                    .Replace("$}", string.Empty)
                    .ToLowerInvariant();

                if (resxValues.ContainsKey(key))
                {
                    databaseSettingsValue.KeyDisplayName = resxValues[key];
                }
            }
        }

        private ReportResults CompileResults(IEnumerable<SettingsKey> databaseSettingsKeys)
        {

            return new ReportResults()
            {
                Data = new TableResult<SettingsKey>()
                {
                    Name = "Settings Key Values",
                    Rows = databaseSettingsKeys
                },
                Status = ReportResultsStatus.Information,
                Summary = String.Empty,
                Type = ReportResultsType.Table
            };
        }
    }
}
