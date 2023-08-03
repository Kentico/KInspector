using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.DebugConfigurationAnalysis;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class DebugConfigurationAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public DebugConfigurationAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockCmsFileService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnErrorStatus_When_DebugEnabledInWebConfig()
        {
            // Arrange
            var customWebConfigXml = @"<configuration><system.web><compilation debug=""true"" /></system.web></configuration>";
            ArrangeServices(customWebconfigXml: customWebConfigXml);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error, "When debug is enabled in the web.config, the report status should be 'error'");
        }

        [Test]
        public void Should_ReturnErrorStatus_When_TraceEnabledInWebConfig()
        {
            // Arrange
            var customWebConfigXml = @"<configuration><system.web><compilation debug=""falue"" /><trace enabled=""true"" /></system.web></configuration>";
            ArrangeServices(customWebconfigXml: customWebConfigXml);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error, "When trace is enabled in the web.config, the report status should be 'error'");
        }

        [Test]
        public void Should_ReturnInformationStatus_When_ResultsAreClean()
        {
            // Arrange
            ArrangeServices();

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Information, "When the results are clean, the report status should be 'information'");
        }

        [Test]
        public void Should_ReturnWarningStatus_When_AnyDatabaseSettingIsTrueAndNotTheDefaultValue()
        {
            // Arrange
            var settingsKey = new SettingsKey("CMSDebugEverything", "Enable all debugs", true, false);
            ArrangeServices(customDatabaseSettingsValues: new SettingsKey[] { settingsKey });

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Warning, "When any database setting is set to true and that isn't the default value, the report status should be 'warning'");
        }

        private void AddDefaultDatabaseSettingsKeyValues(List<SettingsKey> results)
        {
            var defaultDatabaseSettingsKeyValues = new List<SettingsKey>();
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugAnalytics", "Enable web analytics debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugCache", "Enable cache access debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugEverything", "Enable all debugs", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugEverythingEverywhere", "Debug everything everywhere", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugFiles", "Enable IO operation debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugHandlers", "Enable handlers debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugImportExport", "Debug Import/Export", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugMacros", "Enable macro debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugOutput", "Enable output debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugRequests", "Enable request debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugResources", "Debug resources", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugScheduler", "Debug scheduler", true, true));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugSecurity", "Enable security debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugSQLConnections", "Debug SQL connections", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugSQLQueries", "Enable SQL query debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugViewState", "Enable ViewState debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDebugWebFarm", "Enable web farm debug", false, false));
            defaultDatabaseSettingsKeyValues.Add(new SettingsKey("CMSDisableDebug", "Disable debugging", false, false));

            foreach (var settingsKey in defaultDatabaseSettingsKeyValues)
            {
                var keyNameMatchCount = results.Count(x => x.KeyName == settingsKey.KeyName);
                if (keyNameMatchCount == 0)
                {
                    results.Add(settingsKey);
                }
            }
        }

        private void ArrangeDatabaseSettingsMethods(SettingsKey[] customDatabaseSettingsValues)
        {
            IEnumerable<SettingsKey> databaseSettingsKeyValuesResults = GetDatabaseSettingsKeyValuesResults(customDatabaseSettingsValues);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SettingsKey>(Scripts.GetDebugSettingsValues))
                .Returns(databaseSettingsKeyValuesResults);
        }

        private void ArrangeResourceStringsMethods()
        {
            _mockCmsFileService
                .Setup(p => p.GetResourceStringsFromResx(_mockInstance.Path, DefaultKenticoPaths.PrimaryResxFile))
                .Returns(new Dictionary<string, string>());
        }

        private void ArrangeServices(SettingsKey[] customDatabaseSettingsValues = null, string customWebconfigXml = null)
        {
            ArrangeDatabaseSettingsMethods(customDatabaseSettingsValues);
            ArrangeResourceStringsMethods();
            ArrangeWebConfigMethods(customWebconfigXml);
        }

        private void ArrangeWebConfigMethods(string customWebconfigXml)
        {
            var webConfig = new XmlDocument();
            var defaultWebConfigXml = @"<configuration><system.web><compilation debug=""false"" /></system.web></configuration>";
            var webconfigXml = !string.IsNullOrWhiteSpace(customWebconfigXml) ? customWebconfigXml : defaultWebConfigXml;
            webConfig.LoadXml(webconfigXml);

            _mockCmsFileService
                .Setup(p => p.GetXmlDocument(_mockInstance.Path, DefaultKenticoPaths.WebConfigFile))
                .Returns(webConfig);
        }

        private List<SettingsKey> GetDatabaseSettingsKeyValuesResults(SettingsKey[] customSettingsKeyValues = null)
        {
            var results = new List<SettingsKey>();

            if (customSettingsKeyValues != null)
            {
                results.AddRange(customSettingsKeyValues);
            }

            AddDefaultDatabaseSettingsKeyValues(results);

            return results;
        }
    }
}