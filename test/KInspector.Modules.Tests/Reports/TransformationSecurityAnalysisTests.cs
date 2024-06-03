using KInspector.Core.Constants;
using KInspector.Tests.Common.Helpers;
using KInspector.Reports.TransformationSecurityAnalysis;
using KInspector.Reports.TransformationSecurityAnalysis.Models;
using KInspector.Reports.TransformationSecurityAnalysis.Models.Data;
using KInspector.Reports.TransformationSecurityAnalysis.Models.Results;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class TransformationSecurityAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<PageDto> CleanPageDtoTable => new List<PageDto>
        {
            new()
            {
                DocumentName = "Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/page-using-template",
                DocumentPageTemplateID = 1
            },
            new()
            {
                DocumentName = "Another Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/another/page-using-template",
                DocumentPageTemplateID = 1
            },
            new()
            {
                DocumentName = "Page Using Text Page Template",
                DocumentCulture = "es-ES",
                NodeAliasPath = "/path/to/page-using-text-template",
                DocumentPageTemplateID = 2
            }
        };

        private IEnumerable<PageTemplateDto> CleanPageTemplateDtoTable => new List<PageTemplateDto>
        {
            new()
            {
                PageTemplateID = 1,
                PageTemplateWebParts = FromFile(@"Reports\TestData\CMS_PageTemplate\PageTemplateWebParts\CleanAscx.xml"),
                PageTemplateCodeName = "PageTemplateASCX"
            },
            new()
            {
                PageTemplateID = 2,
                PageTemplateWebParts = FromFile(@"Reports\TestData\CMS_PageTemplate\PageTemplateWebParts\CleanText.xml"),
                PageTemplateCodeName = "PageTemplateText"
            }
        };

        private IEnumerable<TransformationDto> CleanTransformationDtoTable => new List<TransformationDto>
        {
            new()
            {
                TransformationName = "ASCXTransformation",
                TransformationCode = FromFile(@"Reports\TestData\CMS_Transformation\TransformationCode\CleanASCX.txt"),
                ClassName = "PageType1",
                Type = TransformationType.ASCX
            },
            new()
            {
                TransformationName = "JQueryTransformation",
                TransformationCode = FromFile(@"Reports\TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType1",
                Type = TransformationType.JQuery
            },
            new()
            {
                TransformationName = "TextTransformation",
                TransformationCode = FromFile(@"Reports\TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType2",
                Type = TransformationType.Text
            }
        };

        public TransformationSecurityAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object, _mockInstanceService.Object, _mockConfigService.Object);
        }

        private static string FromFile(string path)
        {
            return File.ReadAllText(path);
        }

        [Test]
        public async Task Should_ReturnGoodStatusAndGoodSummary_WhenTransformationsHaveNoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CleanTransformationDtoTable);

            // Act
            var results = await mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.GoodSummary?.ToString()));
        }

        [Test]
        public Task Should_ReturnWarningStatus_WhenTransformationsHaveSingleXssQueryHelperIssue() => TestSingleIssue(@"Reports\TestData\CMS_Transformation\TransformationCode\WithXssQueryHelperIssueASCX.txt", (r, d) => d.XssQueryHelper != string.Empty && r.Uses == 2);

        public async Task TestSingleIssue(string transformationCodeFilePath, Func<TransformationResult, dynamic, bool> transformationResultEvaluator)
        {
            var transformationDtoTableWithIssue = new List<TransformationDto>
            {
                    new()
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
            var results = await mockReport.GetResults();
            var issueTypesTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.IssueTypes) ?? false);
            var transformationIssueTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.TransformationsWithIssues) ?? false);
            var transformationUsageTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.TransformationUsage) ?? false);
            var templateUsageTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.TemplateUsage) ?? false);

            // Assert
            Assert.That(issueTypesTable, Is.Not.Null);
            Assert.That(transformationIssueTable, Is.Not.Null);
            Assert.That(transformationUsageTable, Is.Not.Null);
            Assert.That(templateUsageTable, Is.Not.Null);
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
            Assert.That(issueTypesTable?.Rows.Count(), Is.EqualTo(1));
            Assert.That(transformationIssueTable?.Rows.Count(), Is.EqualTo(1));
            Assert.That(transformationIssueTable?.Rows, Has.One.Matches<TransformationResult>(row => transformationResultEvaluator(row, row as dynamic)));
            Assert.That(transformationUsageTable?.Rows.Count(), Is.EqualTo(1));
            Assert.That(templateUsageTable?.Rows.Count(), Is.EqualTo(2));
        }

        private void ArrangeDatabaseService(IEnumerable<TransformationDto> transformationDtoTable)
        {
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetTransformations, transformationDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetPages, CleanPageDtoTable);
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetPageTemplates, "DocumentPageTemplateIDs", CleanPageDtoTable.Select(pageDto => pageDto.DocumentPageTemplateID), CleanPageTemplateDtoTable);
        }
    }
}