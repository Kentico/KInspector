using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.TaskProcessingAnalysis;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Modules.Tests.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class TaskProcessingAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public TaskProcessingAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_ThereAreNoUnprocessedTasks()
        {
            // Arrange
            SetupAllDatabaseQueries();

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Good);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedIntegrationBusTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedIntegrationBusTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, TaskType.IntegrationBusTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedScheduledTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedScheduledTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, TaskType.ScheduledTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedSearchTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedSearchTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, TaskType.SearchTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedStagingTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedStagingTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, TaskType.StagingTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnWarningResult_When_ThereAreUnprocessedWebFarmTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedWebFarmTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, TaskType.WebFarmTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        private static void AssertThatResultsDataIncludesTaskTypeDetails(dynamic data, TaskType taskType)
        {
            var resultsData = (IEnumerable<string>)data;
            var hasTasksListedInResults = resultsData.Any(x => x.Contains(taskType.ToString(), System.StringComparison.InvariantCultureIgnoreCase));

            Assert.That(hasTasksListedInResults, $"'{taskType}' not found in data.");
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