using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.PageTypeFieldAnalysis;
using KenticoInspector.Reports.PageTypeFieldAnalysis.Models;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class PageTypeFieldAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report mockReport;

        private List<CmsPageTypeField> CmsPageTypeFieldsWithoutIssues => new List<CmsPageTypeField>();

        private List<CmsPageTypeField> CmsPageTypeFieldsWithIdenticalNamesAndDifferentDataTypes => new List<CmsPageTypeField>
        {
            new CmsPageTypeField()
            {
                PageTypeCodeName = "DancingGoatMvc.Article",
                FieldName = "ArticleText",
                FieldDataType = "varchar"
            },
            new CmsPageTypeField()
            {
                PageTypeCodeName = "DancingGoatMvc.AboutUs",
                FieldName = "ArticleText",
                FieldDataType = "int"
            }
        };

        public PageTypeFieldAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [TestCase(Category = "Matching fields have save data types", TestName = "Page type fields with matching names and data types produce a good result")]
        public void Should_ReturnGoodResult_When_FieldsHaveNoIssues()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetMatchingCmsPageTypeFieldsWithDifferentDataTypes))
                .Returns(CmsPageTypeFieldsWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        [TestCase(Category = "Matching fields have different data types", TestName = "Page type fields with matching names and different data types produce an information result")]
        public void Should_ReturnInformationResult_When_FieldsWithMatchingNamesHaveDifferentDataTypes()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetMatchingCmsPageTypeFieldsWithDifferentDataTypes))
                .Returns(CmsPageTypeFieldsWithIdenticalNamesAndDifferentDataTypes);

            // Act
            var results = mockReport.GetResults();
            var resultsData = results.Data as TableResult<CmsPageTypeField>;

            // Assert
            Assert.That(resultsData.Rows.Count(), Is.EqualTo(2));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Information));
        }
    }
}