using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public Report(IDatabaseService databaseService, IModuleMetadataService reportMetadataService, IInstanceService instanceService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.PortalEngine,
            ReportTags.Transformations,
            ReportTags.Health,
            ReportTags.Security
        };

        public override ReportResults GetResults()
        {
            var transformationDtos = databaseService.ExecuteSqlFromFile<TransformationDto>(Scripts.GetTransformations);

            var transformationsWithIssues = GetTransformationsWithIssues(transformationDtos);

            var pageDtos = databaseService.ExecuteSqlFromFile<PageDto>(Scripts.GetPages);

            var documentPageTemplateIds = pageDtos
                .Select(pageDto => pageDto.DocumentPageTemplateID);

            var pageTemplateDtos = databaseService.ExecuteSqlFromFile<PageTemplateDto>(Scripts.GetPageTemplates, new { DocumentPageTemplateIDs = documentPageTemplateIds });

            var sites = instanceService
                .GetInstanceDetails(instanceService.CurrentInstance)
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

        private static IEnumerable<PageTemplate> GetPageTemplatesUsingTransformationsWithIssues(IEnumerable<PageTemplate> pageTemplates, IEnumerable<Transformation> transformationsWithIssues)
        {
            foreach (var pageTemplate in pageTemplates)
            {
                foreach (var webPart in pageTemplate.WebParts)
                {
                    foreach (var webPartProperty in webPart.Properties)
                    {
                        var matchingTransformation = transformationsWithIssues
                            .SingleOrDefault(transformation => transformation.FullName == webPartProperty.TransformationFullName);

                        if (matchingTransformation != null)
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

        private ReportResults CompileResults(IEnumerable<PageTemplate> pageTemplates)
        {
            var allIssues = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.WebParts)
                .SelectMany(webPart => webPart.Properties)
                .SelectMany(webPartProperty => webPartProperty.Transformation.Issues);

            if (!allIssues.Any())
            {
                return new ReportResults()
                {
                    Type = ReportResultsType.String,
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var oneIssueOfEachType = allIssues
                .GroupBy(transformationIssue => transformationIssue.IssueType)
                .Select(g => g.First());

            var issueTypes = oneIssueOfEachType
                .Select(transformationIssue => new IssueTypeResult(transformationIssue.IssueType, IssueAnalyzers.DetectedIssueTypes));

            var issueTypesResult = new TableResult<IssueTypeResult>
            {
                Name = Metadata.Terms.TableTitles.IssueTypes,
                Rows = issueTypes
            };

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
                .GroupBy(transformation => transformation.FullName)
                .Select(g => g.First());

            var transformationsResultRows = allTransformations
                .Select(transformation => new TransformationResult(transformation, CountTransformationUses(transformation, pageTemplates), usedIssueTypes))
                .OrderBy(transformationResult => transformationResult.Uses);

            var transformationsResult = new TableResult<TransformationResult>
            {
                Name = Metadata.Terms.TableTitles.TransformationsWithIssues,
                Rows = transformationsResultRows
            };

            var transformationUsageResultRows = pageTemplates
                .SelectMany(AsTransformationUsageResults);

            var transformationUsageResult = new TableResult<TransformationUsageResult>
            {
                Name = Metadata.Terms.TableTitles.TransformationUsage,
                Rows = transformationUsageResultRows
            };

            var templateUsageResultRows = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.Pages)
                .Select(page => new TemplateUsageResult(page));

            var templateUsageResult = new TableResult<TemplateUsageResult>
            {
                Name = Metadata.Terms.TableTitles.TemplateUsage,
                Rows = templateUsageResultRows
            };

            var summaryCount = allTransformations
                .Select(transformation => transformation.Issues)
                .Count();

            var issueTypesAsCsv = string.Join(',', usedIssueTypes
                .Select(issueType => TransformationIssue.ReplaceEachUppercaseLetterWithASpaceAndTheLetter(issueType)));

            return new ReportResults()
            {
                Type = ReportResultsType.TableList,
                Status = ResultsStatus.Warning,
                Summary = Metadata.Terms.WarningSummary.With(new { summaryCount, issueTypesAsCsv }),
                Data = new
                {
                    issueTypesResult,
                    transformationsResult,
                    transformationUsageResult,
                    templateUsageResult
                }
            };
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
                        if (property.Transformation.FullName == transformation.FullName)
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