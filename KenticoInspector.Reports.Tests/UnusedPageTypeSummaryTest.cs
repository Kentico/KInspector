using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.UnusedPageTypeSummary;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models;

using NUnit.Framework;

using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class UnusedPageTypeSummaryTest : AbstractReportTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public UnusedPageTypeSummaryTest(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnInformationStatusAndAllUnusedPageTypes()
        {
            // Arrange
            var unusedPageTypes = GetUnusedPageTypes();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<PageType>(Scripts.GetUnusedPageTypes))
                .Returns(unusedPageTypes);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count == 6);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        public IEnumerable<PageType> GetUnusedPageTypes()
        {
            return new List<PageType>
            {
                new PageType{ ClassDisplayName = "Blog", ClassName = "CMS.Blog" },
                new PageType{ ClassDisplayName = "Blog month", ClassName = "CMS.BlogMonth" },
                new PageType{ ClassDisplayName = "Blog post", ClassName = "CMS.BlogPost" },
                new PageType{ ClassDisplayName = "Chat - Transformation", ClassName = "Chat.Transformations" },
                new PageType{ ClassDisplayName = "Dancing Goat site - Transformations", ClassName = "DancingGoat.Transformations" },
                new PageType{ ClassDisplayName = "E-commerce - Transformations", ClassName = "Ecommerce.Transformations" }
            };
        }
    }
}