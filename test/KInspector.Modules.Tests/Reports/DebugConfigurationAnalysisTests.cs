using KInspector.Core.Constants;
using KInspector.Reports.DebugConfigurationAnalysis;
using KInspector.Reports.DebugConfigurationAnalysis.Models;

using NUnit.Framework;

using System.Xml;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class DebugConfigurationAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public DebugConfigurationAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockCmsFileService.Object, _mockModuleMetadataService.Object, _mockConfigService.Object);
        }

        [Test]
        public async Task Should_ReturnErrorStatus_When_DebugEnabledInWebConfig()
        {
            // Arrange
            var customWebConfigXml = @"<configuration><system.web><compilation debug=""true"" /></system.web></configuration>";
            ArrangeServices(customWebconfigXml: customWebConfigXml);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Error, "When debug is enabled in the web.config, the report status should be 'error'");
        }

        [Test]
        public async Task Should_ReturnErrorStatus_When_TraceEnabledInWebConfig()
        {
            // Arrange
            var customWebConfigXml = @"<configuration><system.web><compilation debug=""falue"" /><trace enabled=""true"" /></system.web></configuration>";
            ArrangeServices(customWebconfigXml: customWebConfigXml);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Error, "When trace is enabled in the web.config, the report status should be 'error'");
        }

        [Test]
        public async Task Should_ReturnInformationStatus_When_ResultsAreClean()
        {
            // Arrange
            ArrangeServices();

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Information, "When the results are clean, the report status should be 'information'");
        }

        [Test]
        public async Task Should_ReturnWarningStatus_When_AnyDatabaseSettingIsTrueAndNotTheDefaultValue()
        {
            // Arrange
            var settingsKey = new SettingsKey("CMSDebugEverything", "Enable all debugs", true, false);
            ArrangeServices(customDatabaseSettingsValues: new SettingsKey[] { settingsKey });

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Warning, "When any database setting is set to true and that isn't the default value, the report status should be 'warning'");
        }

        private void AddDefaultDatabaseSettingsKeyValues(List<SettingsKey> results)
        {
            var defaultDatabaseSettingsKeyValues = new List<SettingsKey>
            {
                new("CMSDebugAnalytics", "Enable web analytics debug", false, false),
                new("CMSDebugCache", "Enable cache access debug", false, false),
                new("CMSDebugEverything", "Enable all debugs", false, false),
                new("CMSDebugEverythingEverywhere", "Debug everything everywhere", false, false),
                new("CMSDebugFiles", "Enable IO operation debug", false, false),
                new("CMSDebugHandlers", "Enable handlers debug", false, false),
                new("CMSDebugImportExport", "Debug Import/Export", false, false),
                new("CMSDebugMacros", "Enable macro debug", false, false),
                new("CMSDebugOutput", "Enable output debug", false, false),
                new("CMSDebugRequests", "Enable request debug", false, false),
                new("CMSDebugResources", "Debug resources", false, false),
                new("CMSDebugScheduler", "Debug scheduler", true, true),
                new("CMSDebugSecurity", "Enable security debug", false, false),
                new("CMSDebugSQLConnections", "Debug SQL connections", false, false),
                new("CMSDebugSQLQueries", "Enable SQL query debug", false, false),
                new("CMSDebugViewState", "Enable ViewState debug", false, false),
                new("CMSDebugWebFarm", "Enable web farm debug", false, false),
                new("CMSDisableDebug", "Disable debugging", false, false)
            };

            foreach (var settingsKey in defaultDatabaseSettingsKeyValues)
            {
                var keyNameMatchCount = results.Count(x => x.KeyName == settingsKey.KeyName);
                if (keyNameMatchCount == 0)
                {
                    results.Add(settingsKey);
                }
            }
        }

        private void ArrangeDatabaseSettingsMethods(SettingsKey[]? customDatabaseSettingsValues)
        {
            IEnumerable<SettingsKey> databaseSettingsKeyValuesResults = GetDatabaseSettingsKeyValuesResults(customDatabaseSettingsValues);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SettingsKey>(Scripts.GetDebugSettingsValues))
                .Returns(Task.FromResult(databaseSettingsKeyValuesResults));
        }

        private void ArrangeResourceStringsMethods()
        {
            _mockCmsFileService
                .Setup(p => p.GetResourceStringsFromResx(_mockInstance.AdministrationPath, DefaultKenticoPaths.PrimaryResxFile))
                .Returns(new Dictionary<string, string>());
        }

        private void ArrangeServices(SettingsKey[]? customDatabaseSettingsValues = null, string? customWebconfigXml = null)
        {
            ArrangeDatabaseSettingsMethods(customDatabaseSettingsValues);
            ArrangeResourceStringsMethods();
            ArrangeWebConfigMethods(customWebconfigXml);
        }

        private void ArrangeWebConfigMethods(string? customWebconfigXml)
        {
            var webConfig = new XmlDocument();
            var defaultWebConfigXml = @"<configuration><system.web><compilation debug=""false"" /></system.web></configuration>";
            var webconfigXml = !string.IsNullOrWhiteSpace(customWebconfigXml) ? customWebconfigXml : defaultWebConfigXml;
            webConfig.LoadXml(webconfigXml);

            _mockCmsFileService
                .Setup(p => p.GetXmlDocument(_mockInstance.AdministrationPath, DefaultKenticoPaths.WebConfigFile))
                .Returns(webConfig);
        }

        private List<SettingsKey> GetDatabaseSettingsKeyValuesResults(SettingsKey[]? customSettingsKeyValues = null)
        {
            var results = new List<SettingsKey>();

            if (customSettingsKeyValues is not null)
            {
                results.AddRange(customSettingsKeyValues);
            }

            AddDefaultDatabaseSettingsKeyValues(results);

            return results;
        }
    }
}