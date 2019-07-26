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

        private const string CleanPageTemplateAscx = @"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanAscx.xml";
        private const string CleanPageTemplateText = @"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanText.xml";

        private const string CleanTransformationAscx = @"TestData\CMS_Transformation\TransformationCode\CleanASCX.txt";
        private const string CleanTransformationText = @"TestData\CMS_Transformation\TransformationCode\CleanText.txt";
        private const string WithXssQueryHelperIssueTransformationAscx = @"TestData\CMS_Transformation\TransformationCode\WithXssQueryHelperIssueASCX.txt";

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
                PageTemplateWebParts = FromFile(CleanPageTemplateAscx),
                PageTemplateCodeName = "PageTemplateASCX"
            },
            new PageTemplateDto()
            {
                PageTemplateID = 2,
                PageTemplateWebParts = FromFile(CleanPageTemplateText),
                PageTemplateCodeName = "PageTemplateText"
            }
        };

        private IEnumerable<TransformationDto> CleanTransformationDtoTable => new List<TransformationDto>
        {
            new TransformationDto()
            {
                TransformationName = "ASCXTransformation",
                TransformationCode = FromFile(CleanTransformationAscx),
                ClassName = "PageType1",
                Type = TransformationType.ASCX
            },
            new TransformationDto()
            {
                TransformationName = "JQueryTransformation",
                TransformationCode = FromFile(CleanTransformationText),
                ClassName = "PageType1",
                Type = TransformationType.JQuery
            },
            new TransformationDto()
            {
                TransformationName = "TextTransformation",
                TransformationCode = FromFile(CleanTransformationText),
                ClassName = "PageType2",
                Type = TransformationType.Text
            }
        };

        private IEnumerable<TransformationDto> XssQueryHelperIssueTransformationDtoTable => new List<TransformationDto>
        {
            new TransformationDto()
            {
                TransformationName = "ASCXTransformation",
                TransformationCode = FromFile(WithXssQueryHelperIssueTransformationAscx),
                ClassName = "PageType1",
                Type = TransformationType.ASCX
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
        public void ShouldReturnNoIssues()
        {
            // Arrange

            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetTransformations, CleanTransformationDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetPages, CleanPageDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetPageTemplates, "DocumentPageTemplateIDs", CleanPageDtoTable.Select(pageDto => pageDto.DocumentPageTemplateID), CleanPageTemplateDtoTable);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.NoIssues.ToString()));
        }

        [Test]
        public void ShouldReturnSingleXssQueryHelperIssue()
        {
            // Arrange
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetTransformations, XssQueryHelperIssueTransformationDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetPages, CleanPageDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetPageTemplates, "DocumentPageTemplateIDs", CleanPageDtoTable.Select(pageDto => pageDto.DocumentPageTemplateID), CleanPageTemplateDtoTable);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));

            Assert.That(GetAnonymousTableResult<TableResult<IssueTypeResult>>(results, "issueTypesResult").Rows.Count(), Is.EqualTo(1));
            Assert.That(GetAnonymousTableResult<TableResult<TransformationResult>>(results, "transformationsResult").Rows.Count(), Is.EqualTo(1));
            Assert.That(GetAnonymousTableResult<TableResult<TransformationResult>>(results, "transformationsResult").Rows, Has.One.Matches<TransformationResult>(r =>
            {
                dynamic row = r;

                return row.XssQueryHelper != string.Empty
                        && r.Uses == 2;
            }
            ));

            Assert.That(GetAnonymousTableResult<TableResult<TransformationUsageResult>>(results, "transformationUsageResult").Rows.Count(), Is.EqualTo(1));

            Assert.That(GetAnonymousTableResult<TableResult<TemplateUsageResult>>(results, "templateUsageResult").Rows.Count(), Is.EqualTo(2));
        }

        private static TResult GetAnonymousTableResult<TResult>(ReportResults results, string resultName)
        {
            return results.Data.GetType().GetProperty(resultName).GetValue(results.Data);
        }
    }
}