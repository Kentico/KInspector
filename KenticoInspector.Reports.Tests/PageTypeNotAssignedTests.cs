﻿using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.PageTypeNotAssignedToSite;
using KenticoInspector.Reports.PageTypeNotAssignedToSite.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class PageTypeNotAssignedTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public PageTypeNotAssignedTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnListOfUnassignedPageTypes_WhenSomeAreFound()
        {
            // Arrange
            var unassignedPageTypes = GetListOfUnassignedPageTypes();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<UnassignedPageTypes>(Scripts.PageTypeNotAssigned))
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
            var unassignedPageTypes = new List<UnassignedPageTypes>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<UnassignedPageTypes>(Scripts.PageTypeNotAssigned))
                .Returns(unassignedPageTypes);
            // Act
            var results = _mockReport.GetResults();
            // Assert
            Assert.That(results.Data.Rows.Count == 0);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        private IEnumerable<UnassignedPageTypes> GetListOfUnassignedPageTypes()
        {
            return new List<UnassignedPageTypes>
            {
                new UnassignedPageTypes
                {
                    ClassName = "DancingGoatMvc.Article",
                    SiteName = "DancingGoatMvc",
                    NodeSiteID = 1,
                    NodeClassID = 5494
                },
                new UnassignedPageTypes
                {
                    ClassName = "DancingGoatMvc.Brewer",
                    SiteName = "DancingGoatMvc",
                    NodeSiteID = 1,
                    NodeClassID = 5477
                },
                new UnassignedPageTypes
                {
                    ClassName = "CMS.News",
                    SiteName = "CorporateSite",
                    NodeSiteID = 2,
                    NodeClassID = 5502
                },
                new UnassignedPageTypes
                {
                    ClassName = "CMS.Office",
                    SiteName = "CorporateSite",
                    NodeSiteID = 2,
                    NodeClassID = 5514
                },
                new UnassignedPageTypes
                {
                    ClassName = "globaltypes.customtype",
                    SiteName = "CorporateSite",
                    NodeSiteID = 2,
                    NodeClassID = 5497
                },
            };
        }
    }
}