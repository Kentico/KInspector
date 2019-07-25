using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.TemplateLayoutAnalysis;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class TemplateLayoutAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public TemplateLayoutAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnListOfIdenticalLayouts_WhenSomeAreFound()
        {
            // Arrange
            var identicalPageLayouts = GetListOfLayouts();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts))
                .Returns(identicalPageLayouts);
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
            var identicalPageLayouts = new List<IdenticalPageLayouts>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts))
                .Returns(identicalPageLayouts);
            // Act
            var results = _mockReport.GetResults();
            // Assert
            Assert.That(results.Data.Rows.Count == 0);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        private IEnumerable<IdenticalPageLayouts> GetListOfLayouts()
        {
            return new List<IdenticalPageLayouts>
            {
                new IdenticalPageLayouts
                {
                    CodeNames = "cms.empty, f3fd44a7-6c41-46cc-b04d-a068c452d092, 93e5724a-6f16-47df-ad12-fc9bdb8f4e81",
                    PageTemplateLayout = "<!-- Container --> <cms:CMSWebPartZone runat=\"server\" ZoneID=\"zoneMain\" /> "
                },
                new IdenticalPageLayouts
                {
                    CodeNames = "cms.forumsadvanceserach, cms.forumswithsearch",
                    PageTemplateLayout = "<!-- Container --> <div class=\"forumSearch\">  <cms:CMSWebPartZone ZoneID=\"zoneLeft\" runat=\"server\" /> </div> "
                },
                new IdenticalPageLayouts
                {
                    CodeNames = "Tree, ObjectTree, Blank, 1b15d44c-e46f-4f82-99af-1b17856e6924, 09f00dba-e441-4d40-bc81-479ed689bfd5, f95abe6f-46d4-4b3c-ab9b-57d18ca1b619, 541d4e51-835c-4eea-8c8e-80b8b843b9bf, 3483629e-920f-4c52-851f-b075aa51e3f9, b04a382c-0d65-44ad-a420-d3f4bd9202e3, 9e2283cd-a5d8-4cb1-9285-ec5e54999d44, 985c5d4a-9919-4114-bf04-fb985716ceb8",
                    PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"zoneA\" runat=\"server\" />"
                },
                new IdenticalPageLayouts
                {
                    CodeNames = "Tabs, HorizontalTabs, VerticalTabs, VerticalTabsWithSiteSelector, HorizontalTabsWithSiteSelector",
                    PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"ZoneContent\" runat=\"server\" />"
                },
                new IdenticalPageLayouts
                {
                    CodeNames = "M_NEdit, Listing, ListingWithGeneralSelector, ListingWithSiteSelector, 23c10bc5-b186-462a-8a56-7a89e03bbad6, Theme, CustomControl",
                    PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"ZoneHeader\" runat=\"server\" ZoneType=\"Header\" />  <cms:CMSWebPartZone ZoneID=\"ZoneContent\" runat=\"server\" />"
                },
            };
        }
    }
}