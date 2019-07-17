using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis
{
    public class TemplateLayoutAnalysisReport : IReport, IWithMetadata<Labels>
    {
        private readonly IDatabaseService databaseService;
        private readonly ILabelService labelService;

        public TemplateLayoutAnalysisReport(IDatabaseService databaseService, ILabelService labelService)
        {
            this.databaseService = databaseService;
            this.labelService = labelService;
        }

        public string Codename => nameof(TemplateLayoutAnalysis);

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version(10, 0),
            new Version(11, 0),
            new Version(12, 0)
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.PortalEngine
        };

        public Metadata<Labels> Metadata => labelService.GetMetadata<Labels>(Codename);

        public ReportResults GetResults()
        {
            var identicalLayouts = databaseService.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts);

            return CompileResults(identicalLayouts);
        }

        private ReportResults CompileResults(IEnumerable<IdenticalPageLayouts> identicalPageLayouts)
        {
            var countIdenticalPageLayouts = identicalPageLayouts.Count();

            var results = new ReportResults
            {
                Status = ReportResultsStatus.Information,
                Type = ReportResultsType.Table,
                Data = new TableResult<dynamic>()
                {
                    Name = Metadata.Labels.IdenticalPageLayouts,
                    Rows = identicalPageLayouts
                }
            };

            if (countIdenticalPageLayouts == 0)
            {
                results.Summary = Metadata.Labels.NoIdenticalPageLayoutsFound;
            }
            else
            {
                results.Summary = Metadata.Labels.CountIdenticalPageLayoutFound.With(new { count = countIdenticalPageLayouts });
            }

            return results;
        }
    }
}