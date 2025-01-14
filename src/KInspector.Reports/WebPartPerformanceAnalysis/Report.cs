using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.WebPartPerformanceAnalysis.Models;

using System.Xml.Linq;

namespace KInspector.Reports.WebPartPerformanceAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService _databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            _databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<Version> IncompatibleVersions => VersionHelper.GetVersionList("13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.PortalEngine,
            ModuleTags.Performance,
            ModuleTags.WebParts,
        };

        public async override Task<ModuleResults> GetResults()
        {
            var affectedTemplates = await _databaseService.ExecuteSqlFromFile<PageTemplate>(Scripts.GetAffectedTemplates);
            var affectedTemplateIds = affectedTemplates.Select(x => x.PageTemplateID).ToArray();
            var affectedDocuments = await _databaseService.ExecuteSqlFromFile<Document>(Scripts.GetDocumentsByPageTemplateIds, new { IDs = affectedTemplateIds });
            var templateAnalysisResults = GetTemplateAnalysisResults(affectedTemplates, affectedDocuments);

            return CompileResults(templateAnalysisResults);
        }

        private List<TemplateSummary> GetTemplateAnalysisResults(IEnumerable<PageTemplate> affectedTemplates, IEnumerable<Document> affectedDocuments)
        {
            var results = new List<TemplateSummary>();
            foreach (var template in affectedTemplates)
            {
                var documents = affectedDocuments.Where(x => x.DocumentPageTemplateID == template.PageTemplateID);
                var affectedWebParts = ExtractWebPartsWithEmptyColumnsProperty(template, documents);

                results.Add(new TemplateSummary()
                {
                    TemplateID = template.PageTemplateID,
                    TemplateName = template.PageTemplateDisplayName,
                    TemplateCodename = template.PageTemplateCodeName,
                    AffectedDocuments = documents,
                    AffectedWebParts = affectedWebParts
                });
            }

            return results;
        }

        private static IEnumerable<WebPartSummary> ExtractWebPartsWithEmptyColumnsProperty(PageTemplate template, IEnumerable<Document> documents)
        {
            var emptyColumnsWebPartProperties = template.PageTemplateWebParts?
                .Descendants("property")
                .Where(x => x.Attribute("name")?.Value == "columns")
                .Where(x => string.IsNullOrWhiteSpace(x.Value)) ?? Enumerable.Empty<XElement>();

            var affectedWebPartsXml = emptyColumnsWebPartProperties.Ancestors("webpart");

            return affectedWebPartsXml.Select(x => new WebPartSummary
            {
                ID = x.Attribute("controlid")?.Value,
                Name = x.Elements("property")?.FirstOrDefault(p => p.Name == "webparttitle")?.Value,
                Type = x.Attribute("type")?.Value,
                TemplateId = template.PageTemplateID,
                Documents = documents
            });
        }

        private ModuleResults CompileResults(IEnumerable<TemplateSummary> templateSummaries)
        {
            var webPartSummaries = templateSummaries.SelectMany(x => x.AffectedWebParts);
            var documentSummaries = templateSummaries.SelectMany(x => x.AffectedDocuments);
            var affectedDocumentCount = documentSummaries.Count();
            var affectedTemplateCount = templateSummaries.Count();
            var affectedWebPartCount = webPartSummaries.Count();
            var summary = Metadata.Terms.Summary?.With(new { affectedDocumentCount, affectedTemplateCount, affectedWebPartCount });
            var status = templateSummaries.Any() ? ResultsStatus.Warning : ResultsStatus.Good;
            var type = templateSummaries.Any() ? ResultsType.TableList : ResultsType.NoResults;

            var result = new ModuleResults
            {
                Status = status,
                Summary = summary,
                Type = type
            };
            if (templateSummaries.Any())
            {
                result.TableResults.Add(new TableResult
                {
                    Name = "Template Summary",
                    Rows = templateSummaries
                });
            }

            if (documentSummaries.Any())
            {
                result.TableResults.Add(new TableResult
                {
                    Name = "Document Summary",
                    Rows = documentSummaries
                });
            }

            if (webPartSummaries.Any())
            {
                result.TableResults.Add(new TableResult
                {
                    Name = "Web Part Summary",
                    Rows = webPartSummaries
                });
            }

            return result;
        }
    }
}