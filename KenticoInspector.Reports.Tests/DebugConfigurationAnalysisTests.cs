using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DebugConfigurationAnalysis;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Xml;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DebugConfigurationAnalysisTests
    {
        private Instance _mockInstance;
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IInstanceService> _mockInstanceService;
        private Mock<ICmsFileService> _mockCmsFileService;
        private Report _mockReport;

        public DebugConfigurationAnalysisTests(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockCmsFileService.Object);
        }

        [Test]
        public void Should_ReturnInformationStatus()
        {
            // Arrange
            IEnumerable<SettingsKey> dbResults = GetCleanResults(5);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SettingsKey>(Scripts.GetDebugSettingsValues))
                .Returns(dbResults);

            _mockCmsFileService
                .Setup(p => p.GetResourceStringsFromResx(_mockInstance.Path, DefaultKenticoPaths.PrimaryResxFile))
                .Returns(new Dictionary<string, string>());

            var webConfig = new XmlDocument();
            webConfig.LoadXml(@"<configuration><system.web><compilation debug=""false"" /></system.web></configuration>");

            _mockCmsFileService
                .Setup(p => p.GetXmlDocument(_mockInstance.Path, DefaultKenticoPaths.WebConfigFile))
                .Returns(webConfig);

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Data.DatabaseSettingsResults.Rows.Count == 5);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        private List<SettingsKey> GetCleanResults(SettingsKey[] customSettingsKeyValues = )
        {
            var results = new List<SettingsKey>();

            for (int i = 0; i < count; i++)
            {
                results.Add(new SettingsKey()
                {
                    KeyDefaultValue = false,
                    KeyDisplayName = $"Value ({i})",
                    KeyName = $"Name{i}",
                    KeyValue = false
                });
            }

            return results;
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
            _mockCmsFileService = MockCmsFileServiceHelper.SetupMockCmsFileService();
        }
    }
}