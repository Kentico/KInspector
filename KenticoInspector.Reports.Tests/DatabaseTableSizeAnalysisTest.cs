using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;
using KenticoInspector.Reports.Tests.Helpers;
using NUnit.Framework;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseTableSizeAnalysisTest : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public DatabaseTableSizeAnalysisTest(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
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
    }
}