using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService _databaseService;
        private readonly IInstanceService _instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string> {
            ReportTags.PortalEngine,
            ReportTags.Performance,
            ReportTags.WebParts,
        };

        public override ReportResults GetResults()
        {
            var affectedTemplates = _databaseService.ExecuteSqlFromFile<PageTemplate>(Scripts.GetAffectedTemplates);
            var affectedTemplateIds = affectedTemplates.Select(x => x.PageTemplateID).ToArray();
            var affectedDocuments = _databaseService.ExecuteSqlFromFile<Document>(Scripts.GetDocumentsByPageTemplateIds, new { IDs = affectedTemplateIds });

            var templateAnalysisResults = GetTemplateAnalysisResults(affectedTemplates, affectedDocuments);

            return CompileResults(templateAnalysisResults);
        }

        private IEnumerable<TemplateSummary> GetTemplateAnalysisResults(IEnumerable<PageTemplate> affectedTemplates, IEnumerable<Document> affectedDocuments)
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

        private IEnumerable<WebPartSummary> ExtractWebPartsWithEmptyColumnsProperty(PageTemplate template, IEnumerable<Document> documents)
        {
            var emptyColumnsWebPartProperties = template.PageTemplateWebParts
                .Descendants("property")
                .Where(x => x.Attribute("name").Value == "columns")
                .Where(x => string.IsNullOrWhiteSpace(x.Value));

            var affectedWebPartsXml = emptyColumnsWebPartProperties.Ancestors("webpart");

            return affectedWebPartsXml.Select(x => new WebPartSummary
            {
                ID = x.Attribute("controlid").Value,
                Name = x.Elements("property").FirstOrDefault(p => p.Name == "webparttitle")?.Value,
                Type = x.Attribute("type").Value,
                TemplateId = template.PageTemplateID,
                Documents = documents
            });
        }

        private ReportResults CompileResults(IEnumerable<TemplateSummary> templateSummaries)
        {
            var templateSummaryTable = new TableResult<TemplateSummary>()
            {
                Name = "Template Summary",
                Rows = templateSummaries
            };

            var webPartSummaries = templateSummaries.SelectMany(x => x.AffectedWebParts);
            var webPartSummaryTable = new TableResult<WebPartSummary>()
            {
                Name = "Web Part Summary",
                Rows = webPartSummaries
            };

            var documentSummaries = templateSummaries.SelectMany(x => x.AffectedDocuments);
            var documentSummaryTable = new TableResult<Document>()
            {
                Name = "Document Summary",
                Rows = documentSummaries
            };

            var data = new
            {
                TemplateSummaryTable = templateSummaryTable,
                WebPartSummaryTable = webPartSummaryTable,
                DocumentSummaryTable = documentSummaryTable
            };

            var affectedDocumentCount = documentSummaries.Count();
            var affectedTemplateCount = templateSummaries.Count();
            var affectedWebPartCount = webPartSummaries.Count();

            var summary = Metadata.Terms.Summary.With(new { affectedDocumentCount, affectedTemplateCount, affectedWebPartCount });

            var status = templateSummaries.Count() > 0 ? ReportResultsStatus.Warning : ReportResultsStatus.Good;

            return new ReportResults
            {
                Status = status,
                Summary = summary,
                Data = data,
                Type = ReportResultsType.TableList
            };
        }
    }
}