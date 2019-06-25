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

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
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

            var databaseSettingsKeys = _databaseService.ExecuteSqlFromFile<SettingsKey>(Scripts.GetDebugSettingsValues);
            var compilationDebugSetting = false;

            return CompileResults(databaseSettingsKeys, compilationDebugSetting);
        }

        private ReportResults CompileResults(IEnumerable<SettingsKey> databaseSettingsKeys, bool compilationDebugSetting)
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
