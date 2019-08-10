using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Reports.TaskProcessingAnalysis;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    public class TaskProcessingAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public TaskProcessingAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_ThereAreNoUnprocessedTasks()
        {
            // Arrange
            SetupAllDatabaseQueries();

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedIntegrationBusTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedIntegrationBusTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountIntegrationBusTask);
            Assert.That(results.Status == ReportResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedScheduledTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedScheduledTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountScheduledTask);
            Assert.That(results.Status == ReportResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedSearchTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedSearchTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountSearchTask);
            Assert.That(results.Status == ReportResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedStagingTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedStagingTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountStagingTask);
            Assert.That(results.Status == ReportResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedWebFarmTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedWebFarmTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountWebFarmTask);
            Assert.That(results.Status == ReportResultsStatus.Warning);
        }

        private static void AssertThatResultsDataIncludesTaskTypeDetails(IList<Result> data, Term term)
        {
            Assert.That(data.Select(x => (string)x), Has.One.Contains(term.ToString()));
        }

        private void SetupAllDatabaseQueries(
                    int unprocessedIntegrationBusTasks = 0,
            int unprocessedScheduledTasks = 0,
            int unprocessedSearchTasks = 0,
            int unprocessedStagingTasks = 0,
            int unprocessedWebFarmTasks = 0
        )
        {
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedIntegrationBusTasks))
                .Returns(unprocessedIntegrationBusTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedScheduledTasks))
                .Returns(unprocessedScheduledTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedSearchTasks))
                .Returns(unprocessedSearchTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedStagingTasks))
                .Returns(unprocessedStagingTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedWebFarmTasks))
                .Returns(unprocessedWebFarmTasks);
        }
    }
}