using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis;
using KenticoInspector.Reports.Tests.MockHelpers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseTableSizeAnalysisTest
    {
        private Instance _mockInstance;
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IInstanceService> _mockInstanceService;
        private Report _mockReport;

        public DatabaseTableSizeAnalysisTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object);
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
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Data.Rows.Count == 25);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        private List<DatabaseTableSizeResult> GetCleanResults()
        {
            var results = new List<DatabaseTableSizeResult>();
            for(var i = 0; i < 25; i++)
            {
                results.Add(new DatabaseTableSizeResult() { TableName = $"table {i}", Rows =  i, BytesPerRow = i, SizeInMB = i });
            }

            return results;
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
        }
    }
}
