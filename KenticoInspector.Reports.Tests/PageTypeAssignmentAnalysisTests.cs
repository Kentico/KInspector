using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
        public void Should_ReturnListOfUnassignedPageTypes_When_SomeAreFound()
        {
            // Arrange
            var unassignedPageTypes = GetListOfUnassignedPageTypes();
            ArrangeDatabaseCalls(unassignedPageTypes);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.OfType<TableResult<PageType>>().First().Rows.Count, Is.GreaterThan(0));
            Assert.That(results.Status == ReportResultsStatus.Warning, $"Expected Warning status, got {results.Status} status");
        }

        [Test]
        public void Should_ReturnEmptyListOfIdenticalLayouts_When_NoneFound()
        {
            // Arrange
            ArrangeDatabaseCalls();

            // Act
            var results = _mockReport.GetResults();
            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good, $"Expected Good status, got {results.Status} status");
        }

        private void ArrangeDatabaseCalls(IEnumerable<PageType> unassignedPageTypes = null)
        {
            unassignedPageTypes = unassignedPageTypes ?? new List<PageType>();
            _mockDatabaseService
               .Setup(p => p.ExecuteSqlFromFile<PageType>(Scripts.GetPageTypesNotAssignedToSite))
               .Returns(unassignedPageTypes);
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