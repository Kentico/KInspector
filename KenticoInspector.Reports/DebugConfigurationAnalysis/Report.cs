using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.DebugConfigurationAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService _databaseService;
        private readonly IInstanceService _instanceService;
        private readonly ICmsFileService _cmsFileService;

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            ICmsFileService cmsFileService,
            IReportMetadataService reportMetadataService
        ) : base(reportMetadataService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
            _cmsFileService = cmsFileService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
           ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var instance = _instanceService.CurrentInstance;
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
                Summary = string.Empty,
                Type = ReportResultsType.TableList
            };

            AnalyzeDatabaseSettingsResults(results, databaseSettingsKeys);
            AnalyzeWebConfigSettings(results, isCompilationDebugEnabled, isTraceEnabled);

            return results;
        }

        private void AnalyzeWebConfigSettings(ReportResults results, bool isCompilationDebugEnabled, bool isTraceEnabled)
        {
            var isDebugOrTraceEnabledInWebConfig = isCompilationDebugEnabled || isTraceEnabled;
            if (isDebugOrTraceEnabledInWebConfig)
            {
                results.Status = ReportResultsStatus.Error;

                var enabledSettingsText = isCompilationDebugEnabled ? "`Debug`" : string.Empty;
                enabledSettingsText += isCompilationDebugEnabled && isTraceEnabled ? " &amp; " : string.Empty;
                enabledSettingsText += isTraceEnabled ? "`Trace`" : string.Empty;
                results.Summary += Metadata.Terms.WebConfig.Summary.With(new { enabledSettingsText });
            }

            var webconfigSettingsValues = new List<SettingsKey>();
            webconfigSettingsValues.Add(new SettingsKey("Debug", Metadata.Terms.WebConfig.DebugKeyDisplayName, isCompilationDebugEnabled, false));
            webconfigSettingsValues.Add(new SettingsKey("Trace", Metadata.Terms.WebConfig.TraceKeyDisplayName, isTraceEnabled, false));

            results.Data.WebConfigSettingsResults = new TableResult<SettingsKey>()
            {
                Name = Metadata.Terms.WebConfig.OverviewTableHeader,
                Rows = webconfigSettingsValues
            };
        }

        private void AnalyzeDatabaseSettingsResults(ReportResults results, IEnumerable<SettingsKey> databaseSettingsKeys)
        {
            var explicitlyEnabledSettings = databaseSettingsKeys.Where(x => x.KeyValue && !x.KeyDefaultValue);
            var explicitlyEnabledSettingsCount = explicitlyEnabledSettings.Count();
            if (explicitlyEnabledSettingsCount > 0)
            {
                if (results.Status != ReportResultsStatus.Error)
                {
                    results.Status = ReportResultsStatus.Warning;
                }

                results.Summary += Metadata.Terms.Database.Summary.With(new { explicitlyEnabledSettingsCount });

                results.Data.DatabaseSettingsEnabledNotByDefaultResults = new TableResult<SettingsKey>()
                {
                    Name = Metadata.Terms.Database.ExplicitlyEnabledSettingsTableHeader,
                    Rows = explicitlyEnabledSettings
                };
            }

            results.Data.AllDatabaseSettings = new TableResult<SettingsKey>()
            {
                Name = Metadata.Terms.Database.OverviewTableHeader,
                Rows = databaseSettingsKeys
            };
        }
    }
}