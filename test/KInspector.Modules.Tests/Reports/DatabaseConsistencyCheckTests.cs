using KInspector.Core.Constants;
using KInspector.Reports.DatabaseConsistencyCheck;
using KInspector.Reports.DatabaseConsistencyCheck.Models;

using NUnit.Framework;

using System.Data;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class DatabaseConsistencyCheckTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public DatabaseConsistencyCheckTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_ReturnGoodStatus_When_ResultsEmpty()
        {
            // Arrange
            var emptyResult = new DataTable();
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(Task.FromResult(emptyResult));
#pragma warning restore 0618
            // Act
            var results = await _mockReport.GetResults();

            //Assert
            Assert.That(results.Status == ResultsStatus.Good);
        }

        [Test]
        public async Task Should_ReturnErrorStatus_When_ResultsNotEmpty()
        {
            // Arrange
            var result = new DataTable();
            result.Columns.Add("TestColumn");
            result.Rows.Add("value");

# pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(Task.FromResult(result));
# pragma warning restore 0618

            // Act
            var results = await _mockReport.GetResults();

            //Assert
            Assert.That(results.Status == ResultsStatus.Error);
        }
    }
}