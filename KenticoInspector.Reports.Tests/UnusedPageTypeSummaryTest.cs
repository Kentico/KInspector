using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.Tests.Helpers;
using KenticoInspector.Reports.UnusedPageTypeSummary;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class UnusedPageTypeSummaryTest
    {
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<ILabelService> _mockLabelService;
        private UnusedPageTypeSummaryReport _mockReport;

        public UnusedPageTypeSummaryTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);

            _mockLabelService = MockLabelServiceHelper.GetlabelService();

            _mockReport = new UnusedPageTypeSummaryReport(_mockDatabaseService.Object, _mockLabelService.Object);

            MockLabelServiceHelper.SetuplabelService<Labels>(_mockLabelService, _mockReport);
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

        private void InitializeCommonMocks(int majorVersion)
        {
            var mockInstance = MockInstances.Get(majorVersion);

            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, mockInstance);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(mockInstance);
        }
    }
}