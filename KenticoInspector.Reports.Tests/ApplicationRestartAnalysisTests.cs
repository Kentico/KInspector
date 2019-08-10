using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Reports.ApplicationRestartAnalysis;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ApplicationRestartAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        private IEnumerable<ApplicationRestartEvent> RestartEvents => new List<ApplicationRestartEvent>
        {
            new ApplicationRestartEvent
            {
                EventCode = "STARTAPP",
                EventTime = DateTime.Now.AddHours(-1),
                EventMachineName = "Server-01"
            },

            new ApplicationRestartEvent
            {
                EventCode = "ENDAPP",
                EventTime = DateTime.Now.AddHours(-1).AddMinutes(-1),
                EventMachineName = "Server-01"
            }
        };

        public ApplicationRestartAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_DatabaseWithoutEvents()
        {
            // Arrange
            var applicationRestartEvents = new List<ApplicationRestartEvent>();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.ApplicationRestartEvents))
                .Returns(applicationRestartEvents);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Information));
        }

        [Test]
        public void Should_ReturnResult_When_DatabaseWithEvents()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.ApplicationRestartEvents))
                .Returns(RestartEvents);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.OfType<TableResult<ApplicationRestartEvent>>().First().Rows.Count, Is.EqualTo(2));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Information));
        }
    }
}