using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ApplicationRestartAnalysis;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ApplicationRestartAnalysisTests
    {
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<ILabelService> _mockLabelService;
        private Report _mockReport;

        public ApplicationRestartAnalysisTests(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);

            _mockLabelService = MockLabelServiceHelper.GetlabelService();

            _mockReport = new Report(_mockDatabaseService.Object, _mockLabelService.Object);

            MockLabelServiceHelper.SetuplabelService<Labels>(_mockLabelService, _mockReport);
        }

        [Test]
        public void Should_ReturnEmptyResult_When_DatabaseHasNoEvents()
        {
            // Arrange
            var applicationRestartEvents = new List<ApplicationRestartEvent>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.ApplicationRestartEvents))
                .Returns(applicationRestartEvents);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count == 0);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        [Test]
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
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.ApplicationRestartEvents))
                .Returns(applicationRestartEvents);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count == 2);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        [Test]
        public void Should_ReturnTableResultType()
        {
            // Arrange
            var applicationRestartEvents = new List<ApplicationRestartEvent>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.ApplicationRestartEvents))
                .Returns(applicationRestartEvents);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Type == ReportResultsType.Table);
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            var mockInstance = MockInstances.Get(majorVersion);

            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, mockInstance);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(mockInstance);
        }
    }
}