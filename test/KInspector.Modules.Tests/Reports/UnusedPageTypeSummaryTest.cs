using KInspector.Core.Constants;
using KInspector.Reports.UnusedPageTypeSummary;
using KInspector.Reports.UnusedPageTypeSummary.Models;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class UnusedPageTypeSummaryTest : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public UnusedPageTypeSummaryTest(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_ReturnInformationStatusAndAllUnusedPageTypes()
        {
            // Arrange
            var unusedPageTypes = GetUnusedPageTypes();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<PageType>(Scripts.GetUnusedPageTypes))
                .Returns(Task.FromResult(unusedPageTypes));

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 6);
            Assert.That(results.Status == ResultsStatus.Information);
        }

        public IEnumerable<PageType> GetUnusedPageTypes()
        {
            return new List<PageType>
            {
                new() { ClassDisplayName = "Blog", ClassName = "CMS.Blog" },
                new() { ClassDisplayName = "Blog month", ClassName = "CMS.BlogMonth" },
                new() { ClassDisplayName = "Blog post", ClassName = "CMS.BlogPost" },
                new() { ClassDisplayName = "Chat - Transformation", ClassName = "Chat.Transformations" },
                new() { ClassDisplayName = "Dancing Goat site - Transformations", ClassName = "DancingGoat.Transformations" },
                new() { ClassDisplayName = "E-commerce - Transformations", ClassName = "Ecommerce.Transformations" }
            };
        }
    }
}