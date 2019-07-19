using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis
{
    public class Report : IReport
    {
        private readonly IDatabaseService _databaseService;
        private readonly IInstanceService _instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public string Codename => nameof(WebPartPerformanceAnalysis);

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version("10.0"),
            new Version("11.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"<p>Displays list of web parts where 'columns' property is not specified.</p>
<p>Web parts without specified 'columns' property must load all field from the database.</p>
<p>By specifying this property, you can significantly lower the data transmission from database to the server and improve the load times.</p>
<p>For more information, <a href=""https://docs.kentico.com/k12sp/configuring-kentico/optimizing-performance-of-portal-engine-sites/loading-data-efficiently"">see documentation</a>.";

        public string Name => "Web Part Performance Analysis";

        public string ShortDescription => "Shows potential optimization opportunities.";

        public IList<string> Tags => new List<string> {
            ReportTags.PortalEngine,
            ReportTags.Performance,
            ReportTags.WebParts,
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            _databaseService.ConfigureForInstance(instance);

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

            var totalAffectedTemplates = templateSummaries.Count();
            var totalAffectedWebParts = webPartSummaries.Count();
            var totalAffectedDocuments = documentSummaries.Count();

            var summary = $"{totalAffectedWebParts} web part{(totalAffectedWebParts!=1 ? "s" : string.Empty )} on {totalAffectedTemplates} template{(totalAffectedTemplates != 1 ? "s" : string.Empty)} affecting {totalAffectedDocuments} document{(totalAffectedDocuments != 1 ? "s" : string.Empty)}";

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
