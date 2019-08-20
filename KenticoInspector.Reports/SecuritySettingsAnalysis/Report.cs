using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly ICmsFileService cmsFileService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Security,
            ReportTags.Configuration
        };

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            ICmsFileService cmsFileService,
            IReportMetadataService reportMetadataService
            ) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.cmsFileService = cmsFileService;
        }

        public override ReportResults GetResults()
        {
            var securityCmsSettingsKeyNames = typeof(SettingsKeyAnalyzers)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(CmsSettingsKeyResult))
                .Select(method => method.Name);

            var cmsSettingsKeys = databaseService.ExecuteSqlFromFile<CmsSettingsKey>(
                Scripts.GetSecurityCmsSettings,
                new { securityCmsSettingsKeyNames }
                );

            var cmsSettingsKeyResults = GetCmsSettingsKeyResults(cmsSettingsKeys);

            var cmsSettingsCategoryIdsOnPaths = cmsSettingsKeyResults
                .Select(settingsKey => settingsKey.CategoryIDPath)
                .SelectMany(idPath => idPath.Split('/', StringSplitOptions.RemoveEmptyEntries))
                .Select(pathSegment => pathSegment.TrimStart('0'))
                .Distinct();

            var cmsSettingsCategories = databaseService.ExecuteSqlFromFile<CmsSettingsCategory>(
                Scripts.GetCmsSettingsCategories,
                new { cmsSettingsCategoryIdsOnPaths }
                );

            var instancePath = instanceService.CurrentInstance.Path;

            var resxValues = cmsFileService.GetResourceStringsFromResx(instancePath);

            var sites = instanceService
                .GetInstanceDetails(instanceService.CurrentInstance)
                .Sites
                .Append(new Site()
                {
                    Id = 0,
                    Name = Metadata.Terms.GlobalSiteName
                });

            var localizedCmsSettingsKeyResults = GetLocalizedCmsSettingsKeyResults(
                cmsSettingsKeyResults,
                cmsSettingsCategories,
                resxValues,
                sites
                );

            var webConfigXml = cmsFileService.GetXmlDocument(instancePath, DefaultKenticoPaths.WebConfigFile);

            // This conversion is temporary pending standardization of XDocument usage
            var webConfig = ToXDocument(webConfigXml);

            var appSettings = webConfig
                .Descendants("appSettings")
                .Elements()
                .Select(element => new AppSetting(element));

            var appSettingsResults = GetAppSettingsResults(appSettings);

            var systemWebElements = webConfig
                .Descendants("system.web")
                .Elements();

            var systemWebSettingsResults = GetSystemWebSettingsResults(systemWebElements);

            return CompileResults(localizedCmsSettingsKeyResults, appSettingsResults.Concat(systemWebSettingsResults));
        }

        private IEnumerable<CmsSettingsKeyResult> GetCmsSettingsKeyResults(IEnumerable<CmsSettingsKey> cmsSettingsKeys)
        {
            var keyAnalyzersObject = new SettingsKeyAnalyzers(Metadata.Terms);

            var keyAnalyzers = keyAnalyzersObject
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(CmsSettingsKeyResult));

            foreach (var cmsSettingsKey in cmsSettingsKeys)
            {
                var matchingKeyAnalyzer = keyAnalyzers
                    .FirstOrDefault(keyAnalyzer => keyAnalyzer.Name
                        .Equals(cmsSettingsKey.KeyName, StringComparison.InvariantCulture));

                if (matchingKeyAnalyzer != null)
                {
                    var analysisResult = matchingKeyAnalyzer.Invoke(keyAnalyzersObject, new[] { cmsSettingsKey });

                    if (analysisResult is CmsSettingsKeyResult settingsKeyResult)
                    {
                        yield return settingsKeyResult;
                    }
                }
            }
        }

        private static IEnumerable<CmsSettingsKeyResult> GetLocalizedCmsSettingsKeyResults(
            IEnumerable<CmsSettingsKeyResult> cmsSettingsKeyResultsWithRecommendations,
            IEnumerable<CmsSettingsCategory> cmsSettingsCategories,
            IDictionary<string, string> resxValues,
            IEnumerable<Site> sites
            )
        {
            foreach (var cmsSettingsKeyResult in cmsSettingsKeyResultsWithRecommendations)
            {
                cmsSettingsKeyResult.SiteName = sites
                    .FirstOrDefault(site => site.Id == cmsSettingsKeyResult.SiteID)
                    .Name;

                cmsSettingsKeyResult.KeyDisplayName = TryReplaceDisplayName(
                    resxValues,
                    cmsSettingsKeyResult.KeyDisplayName
                    );

                var categoryDisplayNames = cmsSettingsKeyResult.CategoryIDPath
                    .Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Select(pathSegment => pathSegment.TrimStart('0'))
                    .Select(idString => cmsSettingsCategories
                        .First(cmsSettingsCategory => cmsSettingsCategory.CategoryID.ToString() == idString)
                        .CategoryDisplayName)
                    .Select(categoryDisplayName => TryReplaceDisplayName(resxValues, categoryDisplayName));

                cmsSettingsKeyResult.KeyPath = string.Join(" > ", categoryDisplayNames);

                yield return cmsSettingsKeyResult;
            }
        }

        private static string TryReplaceDisplayName(IDictionary<string, string> resxValues, string displayName)
        {
            displayName = displayName
                .Replace("{$", string.Empty)
                .Replace("$}", string.Empty)
                .ToLowerInvariant();

            if (resxValues.TryGetValue(displayName, out string keyName))
            {
                displayName = keyName;
            }

            return displayName;
        }

        private static XDocument ToXDocument(XmlDocument document, LoadOptions options = LoadOptions.None)
        {
            using (var reader = new XmlNodeReader(document))
            {
                return XDocument.Load(reader, options);
            }
        }

        private IEnumerable<WebConfigSettingResult> GetAppSettingsResults(IEnumerable<WebConfigSetting> appSettings)
        {
            var appSettingsAnalyzersObject = new AppSettingsAnalyzers(Metadata.Terms);

            var appSettingsAnalyzers = appSettingsAnalyzersObject
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(WebConfigSettingResult));

            foreach (var appSetting in appSettings)
            {
                var matchingKeyAnalyzer = appSettingsAnalyzers
                    .FirstOrDefault(keyAnalyzer => keyAnalyzer.Name
                        .Equals(appSetting.KeyName, StringComparison.InvariantCulture));

                if (matchingKeyAnalyzer != null)
                {
                    var analysisResult = matchingKeyAnalyzer.Invoke(appSettingsAnalyzersObject, new[] { appSetting });

                    if (analysisResult is WebConfigSettingResult appSettingResult)
                    {
                        yield return appSettingResult;
                    }
                }
            }
        }

        private IEnumerable<WebConfigSettingResult> GetSystemWebSettingsResults(IEnumerable<XElement> systemWebElements)
        {
            var systemWebSettingsAnalyzersObject = new SystemWebSettingsAnalyzers(Metadata.Terms);

            var systemWebSettingsAnalyzers = systemWebSettingsAnalyzersObject
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(WebConfigSettingResult));

            foreach (var systemWebElement in systemWebElements)
            {
                foreach (var systemWebSettingsAnalyzer in systemWebSettingsAnalyzers)
                {
                    var analysisResult = systemWebSettingsAnalyzer.Invoke(systemWebSettingsAnalyzersObject, new[] { systemWebElement });

                    if (analysisResult is WebConfigSettingResult systemWebSettingResult)
                    {
                        yield return systemWebSettingResult;
                    }
                }
            }
        }

        private ReportResults CompileResults(
            IEnumerable<CmsSettingsKeyResult> cmsSettingsKeyResults,
            IEnumerable<WebConfigSettingResult> webConfigSettingsResults
            )
        {
            if (!cmsSettingsKeyResults.Any() && !webConfigSettingsResults.Any())
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var errorReportResults = new ReportResults
            {
                Type = ReportResultsType.TableList,
                Status = ReportResultsStatus.Error
            };

            var cmsSettingsKeyResultsCount = IfAnyAddTableResult(
                errorReportResults.Data,
                cmsSettingsKeyResults,
                Metadata.Terms.TableTitles.AdminSecuritySettings
                );

            var webConfigSettingsResultsCount = IfAnyAddTableResult(
                errorReportResults.Data,
                webConfigSettingsResults,
                Metadata.Terms.TableTitles.WebConfigSecuritySettings
                );

            errorReportResults.Summary = Metadata.Terms.Summaries.Error.With(new
            {
                cmsSettingsKeyResultsCount,
                webConfigSettingsResultsCount
            });

            return errorReportResults;
        }

        private static int IfAnyAddTableResult<T>(dynamic data, IEnumerable<T> results, Term tableNameTerm)
        {
            if (results.Any())
            {
                var tableResult = new TableResult<T>
                {
                    Name = tableNameTerm,
                    Rows = results
                };

                IDictionary<string, object> dictionaryData = data;

                dictionaryData.Add(tableNameTerm, tableResult);
            }

            return results.Count();
        }
    }
}