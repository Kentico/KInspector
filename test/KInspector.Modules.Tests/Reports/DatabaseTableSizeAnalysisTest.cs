using KInspector.Core.Constants;
using KInspector.Reports.DatabaseTableSizeAnalysis;
using KInspector.Reports.DatabaseTableSizeAnalysis.Models;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class DatabaseTableSizeAnalysisTest : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public DatabaseTableSizeAnalysisTest(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_ReturnInformationStatus()
        {
            // Arrange
            IEnumerable<DatabaseTableSizeResult> dbResults = GetCleanResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTableSizeResult>(Scripts.GetTop25LargestTables))
                .Returns(Task.FromResult(dbResults));

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 25);
            Assert.That(results.Status == ResultsStatus.Information);
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