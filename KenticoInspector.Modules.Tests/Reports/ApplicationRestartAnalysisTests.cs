using System;
using System.Collections.Generic;

using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.ApplicationRestartAnalysis;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models.Data;

using NUnit.Framework;

namespace KenticoInspector.Modules.Tests.Reports
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
            new CmsEventLog
            {
                EventCode = "STARTAPP",
                EventTime = DateTime.Now.AddHours(-1),
                EventMachineName = "Server-01"
            },

            new CmsEventLog
            {
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
        public void Should_ReturnGoodResult_When_DatabaseWithoutEvents()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(CmsEventsWithoutStartAndEndCodes);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
        }

        [TestCase(Category = "One restart event", TestName = "Database with events produces an information result and lists two events")]
        public void Should_ReturnResult_When_DatabaseWithRestartEvents()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(CmsEventsWithStartAndEndCodes);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Type, Is.EqualTo(ResultsType.Table));
            Assert.That(results.Data.Rows.Count, Is.EqualTo(2));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Information));
        }
    }
}