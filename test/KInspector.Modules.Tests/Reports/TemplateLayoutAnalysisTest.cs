﻿using KInspector.Core.Constants;
using KInspector.Reports.TemplateLayoutAnalysis;
using KInspector.Reports.TemplateLayoutAnalysis.Models;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class TemplateLayoutAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public TemplateLayoutAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_ReturnListOfIdenticalLayouts_WhenSomeAreFound()
        {
            // Arrange
            var identicalPageLayouts = GetListOfLayouts();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts))
                .Returns(Task.FromResult(identicalPageLayouts));

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 5);
            Assert.That(results.Status == ResultsStatus.Information);
        }

        [Test]
        public async Task Should_ReturnEmptyListOfIdenticalLayouts_WhenNoneFound()
        {
            // Arrange
            var identicalPageLayouts = Enumerable.Empty<IdenticalPageLayouts>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts))
                .Returns(Task.FromResult(identicalPageLayouts));

            // Act
            var results = await _mockReport.GetResults();

            // Assert
            Assert.That(!results.TableResults.Any());
            Assert.That(results.Status == ResultsStatus.Information);
        }

        private IEnumerable<IdenticalPageLayouts> GetListOfLayouts()
        {
            return new List<IdenticalPageLayouts>
            {
                new() {
                    CodeNames = "cms.empty, f3fd44a7-6c41-46cc-b04d-a068c452d092, 93e5724a-6f16-47df-ad12-fc9bdb8f4e81",
                    PageTemplateLayout = "<!-- Container --> <cms:CMSWebPartZone runat=\"server\" ZoneID=\"zoneMain\" /> "
                },
                new() {
                    CodeNames = "cms.forumsadvanceserach, cms.forumswithsearch",
                    PageTemplateLayout = "<!-- Container --> <div class=\"forumSearch\">  <cms:CMSWebPartZone ZoneID=\"zoneLeft\" runat=\"server\" /> </div> "
                },
                new() {
                    CodeNames = "Tree, ObjectTree, Blank, 1b15d44c-e46f-4f82-99af-1b17856e6924, 09f00dba-e441-4d40-bc81-479ed689bfd5, f95abe6f-46d4-4b3c-ab9b-57d18ca1b619, 541d4e51-835c-4eea-8c8e-80b8b843b9bf, 3483629e-920f-4c52-851f-b075aa51e3f9, b04a382c-0d65-44ad-a420-d3f4bd9202e3, 9e2283cd-a5d8-4cb1-9285-ec5e54999d44, 985c5d4a-9919-4114-bf04-fb985716ceb8",
                    PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"zoneA\" runat=\"server\" />"
                },
                new() {
                    CodeNames = "Tabs, HorizontalTabs, VerticalTabs, VerticalTabsWithSiteSelector, HorizontalTabsWithSiteSelector",
                    PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"ZoneContent\" runat=\"server\" />"
                },
                new() {
                    CodeNames = "M_NEdit, Listing, ListingWithGeneralSelector, ListingWithSiteSelector, 23c10bc5-b186-462a-8a56-7a89e03bbad6, Theme, CustomControl",
                    PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"ZoneHeader\" runat=\"server\" ZoneType=\"Header\" />  <cms:CMSWebPartZone ZoneID=\"ZoneContent\" runat=\"server\" />"
                },
            };
        }
    }
}