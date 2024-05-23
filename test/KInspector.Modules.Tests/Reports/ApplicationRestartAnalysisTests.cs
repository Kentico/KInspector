using KInspector.Core.Constants;
using KInspector.Reports.ApplicationRestartAnalysis;
using KInspector.Reports.ApplicationRestartAnalysis.Models;
using KInspector.Reports.ApplicationRestartAnalysis.Models.Data;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class ApplicationRestartAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        private IEnumerable<CmsEventLog> CmsEventsWithoutStartAndEndCodes => new List<CmsEventLog>();

        private IEnumerable<CmsEventLog> CmsEventsWithStartAndEndCodes => new List<CmsEventLog>
        {
            new() {
                EventCode = "STARTAPP",
                EventTime = DateTime.Now.AddHours(-1),
                EventMachineName = "Server-01"
            },

            new() {
                EventCode = "ENDAPP",
                EventTime = DateTime.Now.AddHours(-1).AddMinutes(-1),
                EventMachineName = "Server-01"
            }
        };

        public ApplicationRestartAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [TestCase(Category = "No events", TestName = "Database without events produces a good result")]
        public async Task Should_ReturnGoodResult_When_DatabaseWithoutEvents()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(Task.FromResult(CmsEventsWithoutStartAndEndCodes));

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
        }

        [TestCase(Category = "One restart event", TestName = "Database with events produces an information result and lists two events")]
        public async Task Should_ReturnResult_When_DatabaseWithRestartEvents()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(Task.FromResult(CmsEventsWithStartAndEndCodes));

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Information));
        }
    }
}