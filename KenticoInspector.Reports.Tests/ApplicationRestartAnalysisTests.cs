using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.Tests.MockHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(9)]
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ApplicationRestartAnalysisTests
    {
        private Mock<IDatabaseService> _mockDatabaseService;

        private Instance _mockInstance;

        private InstanceDetails _mockInstanceDetails;

        private Mock<IInstanceService> _mockInstanceService;

        public ApplicationRestartAnalysisTests(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
        }

        [TestCase]
        public void Should_ReturnEmptyResult_When_DatabaseHasNoEvents()
        {
            // Arrange
            var applicationRestartEvents = new List<ApplicationRestartEvent>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(ApplicationRestartsScripts.GetApplicationRestartEvents))
                .Returns(applicationRestartEvents);
            
            var report = new ApplicationRestartAnalysis(_mockDatabaseService.Object, _mockInstanceService.Object);

            // Act
            var results = report.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Data.Rows.Count == 0);
            Assert.That(results.Status == ReportResultsStatus.Information.ToString());
        }

        [TestCase]
        public void Should_ReturnTableResultType()
        {
            // Arrange
            var applicationRestartEvents = new List<ApplicationRestartEvent>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(ApplicationRestartsScripts.GetApplicationRestartEvents))
                .Returns(applicationRestartEvents);

            var report = new ApplicationRestartAnalysis(_mockDatabaseService.Object, _mockInstanceService.Object);

            // Act
            var results = report.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Type == ReportResultsType.Table.ToString());
        }

        [TestCase]
        public void Should_ReturnResult_When_DatabaseHasEvents()
        {
            // Arrange
            var applicationRestartEvents = new List<ApplicationRestartEvent>();

            applicationRestartEvents.Add(new ApplicationRestartEvent
            {
                EventCode = "STARTAPP",
                EventTime = DateTime.Now.AddHours(-1),
                EventMachineName = "Server-01"
            });

            applicationRestartEvents.Add(new ApplicationRestartEvent
            {
                EventCode = "ENDAPP",
                EventTime = DateTime.Now.AddHours(-1).AddMinutes(-1),
                EventMachineName = "Server-01"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(ApplicationRestartsScripts.GetApplicationRestartEvents))
                .Returns(applicationRestartEvents);

            var report = new ApplicationRestartAnalysis(_mockDatabaseService.Object, _mockInstanceService.Object);

            // Act
            var results = report.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Data.Rows.Count == 2);
            Assert.That(results.Status == ReportResultsStatus.Information.ToString());
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