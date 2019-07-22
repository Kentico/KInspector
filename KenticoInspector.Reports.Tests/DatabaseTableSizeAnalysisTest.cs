using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseTableSizeAnalysisTest
    {
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<ILabelService> _mockLabelService;
        private Report _mockReport;

        public DatabaseTableSizeAnalysisTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);

            _mockLabelService = MockLabelServiceHelper.GetlabelService();

            _mockReport = new Report(_mockDatabaseService.Object, _mockLabelService.Object);

            MockLabelServiceHelper.SetuplabelService<Labels>(_mockLabelService, _mockReport);
        }

        [Test]
        public void Should_ReturnInformationStatus()
        {
            // Arrange
            IEnumerable<DatabaseTableSizeResult> dbResults = GetCleanResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTableSizeResult>(Scripts.GetTop25LargestTables))
                .Returns(dbResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count == 25);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        private List<DatabaseTableSizeResult> GetCleanResults()
        {
            var results = new List<DatabaseTableSizeResult>();
            for (var i = 0; i < 25; i++)
            {
                results.Add(new DatabaseTableSizeResult() { TableName = $"table {i}", Rows = i, BytesPerRow = i, SizeInMB = i });
            }

            return results;
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            var mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, mockInstance);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(mockInstance);
        }
    }
}