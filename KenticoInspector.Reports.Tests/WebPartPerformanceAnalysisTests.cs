using System;
using System.Collections.Generic;
using System.Linq;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.Tests.Helpers;
using KenticoInspector.Reports.WebPartPerformanceAnalysis;
using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models;
using Moq;
using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    public class WebPartPerformanceAnalysisTest : AbstractReportTest
    {
        private Report _mockReport;

        public WebPartPerformanceAnalysisTest(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object);
        }

        [Test]
        public void Should_ReturnGoodStatus_When_NoWebPartsHaveUnspecifiedColumns()
        {
            // Arrange
            ArrangeAllQueries();

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good, $"Expected status when no web parts have unspecified columns is 'Good' not '{results.Status}'.");
        }

        private void ArrangeAllQueries(List<PageTemplate> affectedTemplates = null)
        {
            affectedTemplates = affectedTemplates ?? new List<PageTemplate>();
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetAffectedTemplates, affectedTemplates);

            var affectedTemplateIds = affectedTemplates.Select(x => x.PageTemplateID);
            var affectedDocuments = new List<Document>();
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetDocumentsByPageTemplateIds, "IDs", affectedTemplateIds, affectedDocuments);

        }
    }
}