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

            var webConfig = _cmsFileService.GetXmlDocument(instance.Path, DefaultKenticoPaths.WebConfigFile);
            var isCompilationDebugEnabled = GetBooleanValueofSectionAttribute(webConfig, "/configuration/system.web/compilation", "debug");
            var isTraceEnabled = GetBooleanValueofSectionAttribute(webConfig, "/configuration/system.web/trace", "enabled");

            return CompileResults(databaseSettingsValues, isCompilationDebugEnabled, isTraceEnabled);
        }

        private static bool GetBooleanValueofSectionAttribute(System.Xml.XmlDocument webConfig, string xpath, string attributeName)
        {
            var valueRaw = webConfig
                .SelectSingleNode(xpath)?
                .Attributes[attributeName]?
                .InnerText;
            var value = false;
            bool.TryParse(valueRaw, out value);
            return value;
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

        private ReportResults CompileResults(IEnumerable<SettingsKey> databaseSettingsKeys, bool isCompilationDebugEnabled, bool isTraceEnabled)
        {
            var results = new ReportResults()
            {
                Status = ReportResultsStatus.Information,
                Summary = String.Empty,
                Type = ReportResultsType.TableList
            };

            var databaseSettingsResults = new TableResult<SettingsKey>()
            {
                Name = "Database Settings",
                Rows = databaseSettingsKeys
            };
            results.Data.DatabaseSettingsResults = databaseSettingsResults;

            var webconfigSettingsValues = new List<SettingsKey>();
            webconfigSettingsValues.Add(new SettingsKey()
            {
                KeyName = "Debug",
                KeyDisplayName = "Compilation Debug",
                KeyDefaultValue = false,
                KeyValue = isCompilationDebugEnabled
            });

            webconfigSettingsValues.Add(new SettingsKey()
            {
                KeyName = "Trace",
                KeyDisplayName = "Trace Enabled",
                KeyDefaultValue = false,
                KeyValue = isTraceEnabled
            });

            var webConfigSettingsResults = new TableResult<SettingsKey>()
            {
                Name = "Web.Config Settings",
                Rows = webconfigSettingsValues
            };

            results.Data.WebConfigSettingsResults = webConfigSettingsResults;

            return results;
        }
    }
}
