using KInspector.Core.Constants;
using KInspector.Reports.PageTypeAssignmentAnalysis;
using KInspector.Reports.PageTypeAssignmentAnalysis.Models;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class PageTypeAssignmentAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public PageTypeAssignmentAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_ReturnListOfUnassignedPageTypes_When_SomeAreFound()
        {
            // Arrange
            var unassignedPageTypes = GetListOfUnassignedPageTypes();
            ArrangeDatabaseCalls(unassignedPageTypes);
            
            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() > 0, "Expected more than 0 page types to be returned");
            Assert.That(results.Status == ResultsStatus.Warning,$"Expected Warning status, got {results.Status} status");
        }

        [Test]
        public async Task Should_ReturnEmptyListOfIdenticalLayouts_When_NoneFound()
        {
            // Arrange
            ArrangeDatabaseCalls();

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(!results.TableResults.Any());
            Assert.That(results.Type, Is.EqualTo(ResultsType.NoResults));
            Assert.That(results.Status == ResultsStatus.Good, $"Expected Good status, got {results.Status} status");
        }

        private void ArrangeDatabaseCalls(IEnumerable<PageType>? unassignedPageTypes = null) {
            unassignedPageTypes ??= new List<PageType>(); 
            _mockDatabaseService
               .Setup(p => p.ExecuteSqlFromFile<PageType>(Scripts.GetPageTypesNotAssignedToSite))
               .Returns(Task.FromResult(unassignedPageTypes));
        }

        private IEnumerable<PageType> GetListOfUnassignedPageTypes()
        {
            return new List<PageType>
            {
                new() {
                    ClassName = "DancingGoatMvc.Article",
                    ClassDisplayName = "Article (MVC)",
                    NodeSiteID = 1,
                    NodeClassID = 5494
                },
                new() {
                    ClassName = "DancingGoatMvc.Brewer",
                    ClassDisplayName = "Brewer (MVC)",
                    NodeSiteID = 1,
                    NodeClassID = 5477
                },
                new() {
                    ClassName = "CMS.News",
                    ClassDisplayName = "News",
                    NodeSiteID = 2,
                    NodeClassID = 5502
                },
                new() {
                    ClassName = "CMS.Office",
                    ClassDisplayName = "Office",
                    NodeSiteID = 2,
                    NodeClassID = 5514
                },
                new() {
                    ClassName = "globaltypes.customtype",
                    ClassDisplayName = "Custom Type",
                    NodeSiteID = 2,
                    NodeClassID = 5497
                },
            };
        }
    }
}