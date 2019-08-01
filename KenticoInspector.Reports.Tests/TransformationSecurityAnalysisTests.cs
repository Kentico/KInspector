using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.Tests.Helpers;
using KenticoInspector.Reports.TransformationSecurityAnalysis;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class TransformationSecurityAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<PageDto> CleanPageDtoTable => new List<PageDto>
        {
            new PageDto()
            {
                DocumentName = "Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/page-using-template",
                DocumentPageTemplateID = 1
            },
            new PageDto()
            {
                DocumentName = "Another Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/another/page-using-template",
                DocumentPageTemplateID = 1
            },
            new PageDto()
            {
                DocumentName = "Page Using Text Page Template",
                DocumentCulture = "es-ES",
                NodeAliasPath = "/path/to/page-using-text-template",
                DocumentPageTemplateID = 2
            }
        };

        private IEnumerable<PageTemplateDto> CleanPageTemplateDtoTable => new List<PageTemplateDto>
        {
            new PageTemplateDto()
            {
                PageTemplateID = 1,
                PageTemplateWebParts = FromFile(@"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanAscx.xml"),
                PageTemplateCodeName = "PageTemplateASCX"
            },
            new PageTemplateDto()
            {
                PageTemplateID = 2,
                PageTemplateWebParts = FromFile(@"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanText.xml"),
                PageTemplateCodeName = "PageTemplateText"
            }
        };

        private IEnumerable<TransformationDto> CleanTransformationDtoTable => new List<TransformationDto>
        {
            new TransformationDto()
            {
                TransformationName = "ASCXTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanASCX.txt"),
                ClassName = "PageType1",
                Type = TransformationType.ASCX
            },
            new TransformationDto()
            {
                TransformationName = "JQueryTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType1",
                Type = TransformationType.JQuery
            },
            new TransformationDto()
            {
                TransformationName = "TextTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType2",
                Type = TransformationType.Text
            }
        };

        public TransformationSecurityAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object, _mockInstanceService.Object);
        }

        private static string FromFile(string path)
        {
            return File.ReadAllText(path);
        }

        [Test]
        public void Should_ReturnGoodStatusAndGoodSummary_WhenTransformationsHaveNoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CleanTransformationDtoTable);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.GoodSummary.ToString()));
        }

        [Test]
        public void Should_ReturnWarningStatus_WhenTransformationsHaveSingleXssQueryHelperIssue() => TestSingleIssue(@"TestData\CMS_Transformation\TransformationCode\WithXssQueryHelperIssueASCX.txt", (r, d) => d.XssQueryHelper != string.Empty && r.Uses == 2);

        public void TestSingleIssue(string transformationCodeFilePath, Func<TransformationResult, dynamic, bool> transformationResultEvaluator)
        {
            var transformationDtoTableWithIssue = new List<TransformationDto>
            {
                    new TransformationDto()
                {
                    TransformationName = "ASCXTransformation",
                    TransformationCode = FromFile(transformationCodeFilePath),
                    ClassName = "PageType1",
                    Type = TransformationType.ASCX
                }
            };

            // Arrange
            ArrangeDatabaseService(transformationDtoTableWithIssue);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));

            Assert.That(GetAnonymousTableResult<TableResult<IssueTypeResult>>(results, "issueTypesResult").Rows.Count(), Is.EqualTo(1));
            Assert.That(GetAnonymousTableResult<TableResult<TransformationResult>>(results, "transformationsResult").Rows.Count(), Is.EqualTo(1));
            Assert.That(GetAnonymousTableResult<TableResult<TransformationResult>>(results, "transformationsResult").Rows, Has.One.Matches<TransformationResult>(row => transformationResultEvaluator(row, row as dynamic)));

            Assert.That(GetAnonymousTableResult<TableResult<TransformationUsageResult>>(results, "transformationUsageResult").Rows.Count(), Is.EqualTo(1));

            Assert.That(GetAnonymousTableResult<TableResult<TemplateUsageResult>>(results, "templateUsageResult").Rows.Count(), Is.EqualTo(2));
        }

        private void ArrangeDatabaseService(IEnumerable<TransformationDto> transformationDtoTable)
        {
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetTransformations, transformationDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetPages, CleanPageDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetPageTemplates, "DocumentPageTemplateIDs", CleanPageDtoTable.Select(pageDto => pageDto.DocumentPageTemplateID), CleanPageTemplateDtoTable);
        }

        private static TResult GetAnonymousTableResult<TResult>(ReportResults results, string resultName)
        {
            return results
                .Data
                .GetType()
                .GetProperty(resultName)
                .GetValue(results.Data);
        }
    }
}