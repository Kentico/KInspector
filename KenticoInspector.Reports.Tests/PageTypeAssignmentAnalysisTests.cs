using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class PageTypeAssignmentAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public PageTypeAssignmentAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnListOfUnassignedPageTypes_WhenSomeAreFound()
        {
            // Arrange
            var unassignedPageTypes = GetListOfUnassignedPageTypes();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<PageType>(Scripts.GetPageTypesNotAssignedToSite))
                .Returns(unassignedPageTypes);
            // Act
            var results = _mockReport.GetResults();
            // Assert
            Assert.That(results.Data.Rows.Count == 5);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        [Test]
        public void Should_ReturnEmptyListOfIdenticalLayouts_WhenNoneFound()
        {
            // Arrange
            var unassignedPageTypes = new List<PageType>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<PageType>(Scripts.GetPageTypesNotAssignedToSite))
                .Returns(unassignedPageTypes);
            // Act
            var results = _mockReport.GetResults();
            // Assert
            Assert.That(results.Data.Rows.Count == 0);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        private IEnumerable<PageType> GetListOfUnassignedPageTypes()
        {
            return new List<PageType>
            {
                new PageType
                {
                    ClassName = "DancingGoatMvc.Article",
                    ClassDisplayName = "Article (MVC)",
                    NodeSiteID = 1,
                    NodeClassID = 5494
                },
                new PageType
                {
                    ClassName = "DancingGoatMvc.Brewer",
                    ClassDisplayName = "Brewer (MVC)",
                    NodeSiteID = 1,
                    NodeClassID = 5477
                },
                new PageType
                {
                    ClassName = "CMS.News",
                    ClassDisplayName = "News",
                    NodeSiteID = 2,
                    NodeClassID = 5502
                },
                new PageType
                {
                    ClassName = "CMS.Office",
                    ClassDisplayName = "Office",
                    NodeSiteID = 2,
                    NodeClassID = 5514
                },
                new PageType
                {
                    ClassName = "globaltypes.customtype",
                    ClassDisplayName = "Custom Type",
                    NodeSiteID = 2,
                    NodeClassID = 5497
                },
            };
        }
    }
}