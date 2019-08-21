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
using KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers;
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
            var cmsSettingsKeysNames = typeof(SettingsKeyAnalyzers)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(CmsSettingsKeyResult))
                .Select(method => method.Name);

            var cmsSettingsKeys = databaseService.ExecuteSqlFromFile<CmsSettingsKey>(
                Scripts.GetSecurityCmsSettings,
                new { cmsSettingsKeysNames }
                );

            var cmsSettingsKeyResults = GetCmsSettingsKeyResults(cmsSettingsKeys);

            var cmsSettingsCategoryIdsOnPaths = cmsSettingsKeyResults
                .SelectMany(cmsSettingsKeyResult => cmsSettingsKeyResult.GetCategoryIdsOnPath())
                .Distinct();

            var cmsSettingsCategories = databaseService.ExecuteSqlFromFile<CmsSettingsCategory>(
                Scripts.GetCmsSettingsCategories,
                new { cmsSettingsCategoryIdsOnPaths }
                );

            var sites = instanceService
                .GetInstanceDetails(instanceService.CurrentInstance)
                .Sites
                .Append(new Site()
                {
                    Id = 0,
                    Name = Metadata.Terms.GlobalSiteName
                });

            var instancePath = instanceService.CurrentInstance.Path;

            var resxValues = cmsFileService.GetResourceStringsFromResx(instancePath);

            var localizedCmsSettingsKeyResults = cmsSettingsKeyResults
                .Select(cmsSettingsKeyResult => new CmsSettingsKeyResult(
                    cmsSettingsKeyResult,
                    cmsSettingsCategories,
                    sites,
                    resxValues
                    ));

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

            var connectionStringElements = webConfig
                .Descendants("connectionStrings")
                .Elements();

            var connectionStringElementsResults = GetConnectionStringsResults(connectionStringElements);

            var webConfigSettingsResults = appSettingsResults
                .Concat(systemWebSettingsResults)
                .Concat(connectionStringElementsResults);

            return CompileResults(localizedCmsSettingsKeyResults, webConfigSettingsResults);
        }

        private IEnumerable<CmsSettingsKeyResult> GetCmsSettingsKeyResults(IEnumerable<CmsSettingsKey> cmsSettingsKeys)
        {
            var analyzersObject = new SettingsKeyAnalyzers(Metadata.Terms);

            foreach (var cmsSettingsKey in cmsSettingsKeys)
            {
                var analysisResult = analyzersObject.GetAnalysis(cmsSettingsKey, cmsSettingsKey.KeyName);

                if (analysisResult is CmsSettingsKeyResult cmsSettingsKeyResult)
                {
                    yield return cmsSettingsKeyResult;
                }
            }
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
            var analyzersObject = new AppSettingAnalyzers(Metadata.Terms);

            foreach (var appSetting in appSettings)
            {
                var analysisResult = analyzersObject.GetAnalysis(appSetting, appSetting.KeyName);

                if (analysisResult is WebConfigSettingResult appSettingResult)
                {
                    yield return appSettingResult;
                }
            }
        }

        private IEnumerable<WebConfigSettingResult> GetSystemWebSettingsResults(IEnumerable<XElement> systemWebElements)
        {
            var analyzersObject = new SystemWebSettingAnalyzers(Metadata.Terms);

            foreach (var systemWebElement in systemWebElements)
            {
                var analysisResult = analyzersObject.GetAnalysis(systemWebElement, systemWebElement.Name.ToString());

                if (analysisResult is WebConfigSettingResult systemWebSettingResult)
                {
                    yield return systemWebSettingResult;
                }
            }
        }

        private IEnumerable<WebConfigSettingResult> GetConnectionStringsResults(IEnumerable<XElement> connectionStringElements)
        {
            var analyzersObject = new ConnectionStringsAnalyzers(Metadata.Terms);

            foreach (var connectionStringElement in connectionStringElements)
            {
                var analysisResult = analyzersObject.GetAnalysis(
                    connectionStringElement,
                    connectionStringElement.Attribute("name")?.Value
                    );

                if (analysisResult is WebConfigSettingResult systemWebSettingResult)
                {
                    yield return systemWebSettingResult;
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