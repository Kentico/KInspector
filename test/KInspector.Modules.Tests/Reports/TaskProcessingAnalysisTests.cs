using KInspector.Core.Constants;
using KInspector.Reports.TaskProcessingAnalysis;
using KInspector.Reports.TaskProcessingAnalysis.Models;
using KInspector.Tests.Common.Helpers;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    [TestFixture(30)]
    [TestFixture(31)]
    public class TaskProcessingAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public TaskProcessingAnalysisTests(int majorVersion) : base(majorVersion)
        {
            var mockInstance = MockInstances.Get(majorVersion) ?? throw new InvalidOperationException($"No instance found with major version {majorVersion}.");
            var mockInstanceDetails = MockInstanceDetails.Get(majorVersion) ?? throw new InvalidOperationException($"No instance details found with major version {majorVersion}.");
            var mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(mockInstance, mockInstanceDetails);
            var mockConfigService = MockConfigServiceHelper.SetupMockConfigService(mockInstance);

            _mockReport = new Report(_mockDatabaseService.Object, mockInstanceService.Object, mockConfigService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_ReturnGoodResult_When_ThereAreNoUnprocessedTasks()
        {
            // Arrange
            SetupAllDatabaseQueries();

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Good);
        }

        [Test]
        public async Task Should_ReturnWarningResult_When_ThereAreUnprocessedIntegrationBusTasks()
        {
            if (_mockInstanceDetails?.AdministrationDatabaseVersion?.Major > 13)
            {
                // Integration bus is not present in XbK
                return;
            }

            // Arrange
            SetupAllDatabaseQueries(unprocessedIntegrationBusTasks: 1);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.StringResults, TaskType.IntegrationBusTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public async Task Should_ReturnWarningResult_When_ThereAreUnprocessedScheduledTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedScheduledTasks: 1);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.StringResults, TaskType.ScheduledTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public async Task Should_ReturnWarningResult_When_ThereAreUnprocessedSearchTasks()
        {
            if (_mockInstanceDetails?.AdministrationDatabaseVersion?.Major > 13)
            {
                // Search tasks not present in XbK
                return;
            }

            // Arrange
            SetupAllDatabaseQueries(unprocessedSearchTasks: 1);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.StringResults, TaskType.SearchTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public async Task Should_ReturnWarningResult_When_ThereAreUnprocessedStagingTasks()
        {
            if (_mockInstanceDetails?.AdministrationDatabaseVersion?.Major > 13)
            {
                // Staging tasks are not present in XbK
                return;
            }

            // Arrange
            SetupAllDatabaseQueries(unprocessedStagingTasks: 1);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.StringResults, TaskType.StagingTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public async Task Should_ReturnWarningResult_When_ThereAreUnprocessedWebFarmTasks()
        {
            // Arrange
            SetupAllDatabaseQueries(unprocessedWebFarmTasks: 1);

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.StringResults, TaskType.WebFarmTask);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        private static void AssertThatResultsDataIncludesTaskTypeDetails(IEnumerable<string> stringResults, TaskType taskType)
        {
            var hasTasksListedInResults = stringResults.Any(x => x.Contains(taskType.ToString(), StringComparison.InvariantCultureIgnoreCase));

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
                .Returns(Task.FromResult(unprocessedIntegrationBusTasks));

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedScheduledTasks))
                .Returns(Task.FromResult(unprocessedScheduledTasks));
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedScheduledTasksXbK))
                .Returns(Task.FromResult(unprocessedScheduledTasks));

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedSearchTasks))
                .Returns(Task.FromResult(unprocessedSearchTasks));

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedStagingTasks))
                .Returns(Task.FromResult(unprocessedStagingTasks));

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedWebFarmTasks))
                .Returns(Task.FromResult(unprocessedWebFarmTasks));
        }
    }
}