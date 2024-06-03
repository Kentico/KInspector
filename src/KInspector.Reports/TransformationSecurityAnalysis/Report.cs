using System.Reflection;

using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.TransformationSecurityAnalysis.Models;
using KInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;
using KInspector.Reports.TransformationSecurityAnalysis.Models.Data;
using KInspector.Reports.TransformationSecurityAnalysis.Models.Results;

namespace KInspector.Reports.TransformationSecurityAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly IConfigService configService;

        public Report(
            IDatabaseService databaseService,
            IModuleMetadataService moduleMetadataService,
            IInstanceService instanceService,
            IConfigService configService
            ) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.configService = configService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<Version> IncompatibleVersions => VersionHelper.GetVersionList("13");

        public override IList<string> Tags => new List<string>
        {
            ModuleTags.Information,
            ModuleTags.PortalEngine,
            ModuleTags.Transformations,
            ModuleTags.Health,
            ModuleTags.Security
        };

        public async override Task<ModuleResults> GetResults()
        {
            var transformationDtos = await databaseService.ExecuteSqlFromFile<TransformationDto>(Scripts.GetTransformations);
            var transformationsWithIssues = GetTransformationsWithIssues(transformationDtos);
            var pageDtos = await databaseService.ExecuteSqlFromFile<PageDto>(Scripts.GetPages);
            var documentPageTemplateIds = pageDtos.Select(pageDto => pageDto.DocumentPageTemplateID);
            var pageTemplateDtos = await databaseService.ExecuteSqlFromFile<PageTemplateDto>(Scripts.GetPageTemplates, new { DocumentPageTemplateIDs = documentPageTemplateIds });
            var sites = instanceService
                .GetInstanceDetails(configService.GetCurrentInstance())
                .Sites;

            var pageTemplates = pageTemplateDtos
                .Select(pageTemplateDto => new PageTemplate(sites, pageDtos, pageTemplateDto))
                .ToList();

            var pageTemplatesUsingTransformationsWithIssues = GetPageTemplatesUsingTransformationsWithIssues(pageTemplates, transformationsWithIssues);

            return CompileResults(pageTemplatesUsingTransformationsWithIssues);
        }

        private IEnumerable<Transformation> GetTransformationsWithIssues(IEnumerable<TransformationDto> transformationDtos)
        {
            var transformations = transformationDtos
                .Select(transformationDto => new Transformation(transformationDto))
                .ToList();

            foreach (var transformation in transformations)
            {
                AnalyzeTransformation(transformation);
            }

            return transformations
                .Where(transformation => transformation
                    .Issues
                    .Any()
                );
        }

        private void AnalyzeTransformation(Transformation transformation)
        {
            var issueAnalyzersObject = new IssueAnalyzers(Metadata.Terms);
            var issueAnalyzerPublicInstanceMethods = issueAnalyzersObject
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(void));

            foreach (var issueAnalyzerPublicInstanceMethod in issueAnalyzerPublicInstanceMethods)
            {
                issueAnalyzerPublicInstanceMethod.Invoke(issueAnalyzersObject, new object[] { transformation });
            }
        }

        private static IEnumerable<PageTemplate> GetPageTemplatesUsingTransformationsWithIssues(
            IEnumerable<PageTemplate> pageTemplates,
            IEnumerable<Transformation> transformationsWithIssues)
        {
            foreach (var pageTemplate in pageTemplates)
            {
                foreach (var webPart in pageTemplate.WebParts)
                {
                    foreach (var webPartProperty in webPart.Properties)
                    {
                        var matchingTransformation = transformationsWithIssues
                            .SingleOrDefault(transformation => transformation.FullName == webPartProperty.TransformationFullName);

                        if (matchingTransformation is not null)
                        {
                            webPartProperty.Transformation = matchingTransformation;
                        }
                    }

                    webPart.RemovePropertiesWithoutTransformations();
                }

                pageTemplate.RemoveWebPartsWithNoProperties();
            }

            return pageTemplates
                .Where(pageTemplate => pageTemplate
                    .WebParts
                    .Any()
                );
        }

        private ModuleResults CompileResults(IEnumerable<PageTemplate> pageTemplates)
        {
            var allIssues = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.WebParts)
                .SelectMany(webPart => webPart.Properties)
                .SelectMany(webPartProperty => webPartProperty.Transformation?.Issues ?? Enumerable.Empty<TransformationIssue>());

            if (!allIssues.Any())
            {
                return new ModuleResults()
                {
                    Type = ResultsType.NoResults,
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var oneIssueOfEachType = allIssues
                .GroupBy(transformationIssue => transformationIssue.IssueType)
                .Select(g => g.First());
            var issueTypes = oneIssueOfEachType
                .Select(transformationIssue => new IssueTypeResult(transformationIssue.IssueType ?? string.Empty, IssueAnalyzers.DetectedIssueTypes));
            var usedIssueTypes = IssueAnalyzers.DetectedIssueTypes
                .Keys
                .Where(issueType => oneIssueOfEachType
                    .Select(issue => issue.IssueType)
                    .Contains(issueType)
                );
            var allTransformations = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.WebParts)
                .SelectMany(webPart => webPart.Properties)
                .Select(webPartProperty => webPartProperty.Transformation)
                .GroupBy(transformation => transformation?.FullName)
                .Select(g => g.First());
            var transformationsResultRows = allTransformations
                .Select(transformation => new TransformationResult(transformation, CountTransformationUses(transformation, pageTemplates), usedIssueTypes))
                .OrderBy(transformationResult => transformationResult.Uses);
            var transformationUsageResultRows = pageTemplates
                .SelectMany(AsTransformationUsageResults);
            var templateUsageResultRows = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.Pages)
                .Select(page => new TemplateUsageResult(page));
            var summaryCount = allTransformations
                .Select(transformation => transformation.Issues)
                .Count();

            var issueTypesAsCsv = string.Join(',', usedIssueTypes
                .Select(issueType => TransformationIssue.ReplaceEachUppercaseLetterWithASpaceAndTheLetter(issueType)));

            var result = new ModuleResults()
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Warning,
                Summary = Metadata.Terms.WarningSummary?.With(new { summaryCount, issueTypesAsCsv })
            };
            result.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitles?.IssueTypes,
                Rows = issueTypes
            });
            result.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitles?.TransformationUsage,
                Rows = transformationUsageResultRows
            });
            result.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitles?.TemplateUsage,
                Rows = templateUsageResultRows
            });
            result.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitles?.TransformationsWithIssues,
                Rows = transformationsResultRows
            });

            return result;
        }

        private static int CountTransformationUses(Transformation transformation, IEnumerable<PageTemplate> pageTemplates)
        {
            var totalCount = 0;

            foreach (var pageTemplate in pageTemplates)
            {
                var templateCount = 0;

                foreach (var webPart in pageTemplate.WebParts)
                {
                    foreach (var property in webPart.Properties)
                    {
                        if (property.Transformation?.FullName == transformation.FullName)
                        {
                            templateCount++;
                        }
                    }
                }

                if (templateCount > 0)
                {
                    templateCount *= pageTemplate.Pages.Count();
                }

                totalCount += templateCount;
            }

            return totalCount;
        }

        private static IEnumerable<TransformationUsageResult> AsTransformationUsageResults(PageTemplate pageTemplate)
        {
            foreach (var webPart in pageTemplate.WebParts)
            {
                foreach (var property in webPart.Properties)
                {
                    yield return new TransformationUsageResult(
                        pageTemplate,
                        webPart,
                        property,
                        property.Transformation
                    );
                }
            }
        }
    }
}