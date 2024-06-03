using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.DebugConfigurationAnalysis.Models;


namespace KInspector.Reports.DebugConfigurationAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICmsFileService _cmsFileService;
        private readonly IConfigService _configService;

        public Report(
            IDatabaseService databaseService,
            ICmsFileService cmsFileService,
            IModuleMetadataService moduleMetadataService,
            IConfigService configService
        ) : base(moduleMetadataService)
        {
            _databaseService = databaseService;
            _cmsFileService = cmsFileService;
            _configService = configService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
           ModuleTags.Health
        };

        public async override Task<ModuleResults> GetResults()
        {
            var instance = _configService.GetCurrentInstance() ?? throw new InvalidOperationException("Current instance not found.");
            var databaseSettingsValues = await _databaseService.ExecuteSqlFromFile<SettingsKey>(Scripts.GetDebugSettingsValues);
            ResolveSettingsDisplayNames(instance, databaseSettingsValues);

            var webConfig = _cmsFileService.GetXmlDocument(instance.AdministrationPath, DefaultKenticoPaths.WebConfigFile) ?? throw new InvalidOperationException("Unable to load instance web.config.");
            var isCompilationDebugEnabled = GetBooleanValueofSectionAttribute(webConfig, "/configuration/system.web/compilation", "debug");
            var isTraceEnabled = GetBooleanValueofSectionAttribute(webConfig, "/configuration/system.web/trace", "enabled");

            return CompileResults(databaseSettingsValues, isCompilationDebugEnabled, isTraceEnabled);
        }

        private static bool GetBooleanValueofSectionAttribute(System.Xml.XmlDocument webConfig, string xpath, string attributeName)
        {
            var valueRaw = webConfig
                .SelectSingleNode(xpath)?
                .Attributes?[attributeName]?
                .InnerText;
            var value = false;
            bool.TryParse(valueRaw, out value);

            return value;
        }

        private void ResolveSettingsDisplayNames(Instance instance, IEnumerable<SettingsKey> databaseSettingsValues)
        {
            var resxValues = _cmsFileService.GetResourceStringsFromResx(instance.AdministrationPath);
            foreach (var databaseSettingsValue in databaseSettingsValues)
            {
                var key = databaseSettingsValue.KeyDisplayName?
                    .Replace("{$", string.Empty)
                    .Replace("$}", string.Empty)
                    .ToLowerInvariant();

                if (key is not null && resxValues.ContainsKey(key))
                {
                    databaseSettingsValue.KeyDisplayName = resxValues[key];
                }
            }
        }

        private ModuleResults CompileResults(IEnumerable<SettingsKey> databaseSettingsKeys, bool isCompilationDebugEnabled, bool isTraceEnabled)
        {
            var results = new ModuleResults()
            {
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.CheckResultsTableForAnyIssues,
                Type = ResultsType.TableList
            };

            AnalyzeDatabaseSettingsResults(results, databaseSettingsKeys);
            AnalyzeWebConfigSettings(results, isCompilationDebugEnabled, isTraceEnabled);

            return results;
        }

        private void AnalyzeWebConfigSettings(ModuleResults results, bool isCompilationDebugEnabled, bool isTraceEnabled)
        {
            var isDebugOrTraceEnabledInWebConfig = isCompilationDebugEnabled || isTraceEnabled;
            if (isDebugOrTraceEnabledInWebConfig)
            {
                results.Status = ResultsStatus.Error;

                var enabledSettingsText = isCompilationDebugEnabled ? "`Debug`" : string.Empty;
                enabledSettingsText += isCompilationDebugEnabled && isTraceEnabled ? " &amp; " : string.Empty;
                enabledSettingsText += isTraceEnabled ? "`Trace`" : string.Empty;
                results.Summary += Metadata.Terms.WebConfig?.Summary?.With(new { enabledSettingsText });
            }

            var webconfigSettingsValues = new List<SettingsKey>
            {
                new("Debug", Metadata.Terms.WebConfig?.DebugKeyDisplayName, isCompilationDebugEnabled, false),
                new("Trace", Metadata.Terms.WebConfig?.TraceKeyDisplayName, isTraceEnabled, false)
            };

            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.WebConfig?.OverviewTableHeader,
                Rows = webconfigSettingsValues
            });
        }

        private void AnalyzeDatabaseSettingsResults(ModuleResults results, IEnumerable<SettingsKey> databaseSettingsKeys)
        {
            var explicitlyEnabledSettings = databaseSettingsKeys.Where(x => x.KeyValue && !x.KeyDefaultValue);
            var explicitlyEnabledSettingsCount = explicitlyEnabledSettings.Count();
            if (explicitlyEnabledSettingsCount > 0)
            {
                if (results.Status != ResultsStatus.Error)
                {
                    results.Status = ResultsStatus.Warning;
                }

                results.Summary += Metadata.Terms.Database?.Summary?.With(new { explicitlyEnabledSettingsCount });

                results.TableResults.Add(new TableResult
                {
                    Name = Metadata.Terms.Database?.ExplicitlyEnabledSettingsTableHeader,
                    Rows = explicitlyEnabledSettings
                });
            }

            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.Database?.OverviewTableHeader,
                Rows = databaseSettingsKeys
            });
        }
    }
}