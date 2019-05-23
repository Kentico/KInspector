using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.UnusedPageTypeSummary;
using KenticoInspector.Reports.Tests.MockHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class UnusedPageTypeSummaryTest
    {
        private Instance _mockInstance;
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IInstanceService> _mockInstanceService;
        private Report _mockReport;

        public UnusedPageTypeSummaryTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object);
        }

        [Test]
        public void Should_ReturnInformationStatus()
        {
            // Arrange
            var unusedTypes = GetListOfPageTypes();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<UnusedPageTypes>(Scripts.GetUnusedPageTypes))
                .Returns(unusedTypes);

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Data.Rows.Count == 6);
            Assert.That(results.Status == ReportResultsStatus.Information);

        }

        public IEnumerable<UnusedPageTypes> GetListOfPageTypes()
        {
            return new List<UnusedPageTypes>
            {
                new UnusedPageTypes{ ClassDisplayName = "Blog", ClassName = "CMS.Blog" },
                new UnusedPageTypes{ ClassDisplayName = "Blog month", ClassName = "CMS.BlogMonth" },
                new UnusedPageTypes{ ClassDisplayName = "Blog post", ClassName = "CMS.BlogPost" },
                new UnusedPageTypes{ ClassDisplayName = "Chat - Transformation", ClassName = "Chat.Transformations" },
                new UnusedPageTypes{ ClassDisplayName = "Dancing Goat site - Transformations", ClassName = "DancingGoat.Transformations" },
                new UnusedPageTypes{ ClassDisplayName = "E-commerce - Transformations", ClassName = "Ecommerce.Transformations" }
            };
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
