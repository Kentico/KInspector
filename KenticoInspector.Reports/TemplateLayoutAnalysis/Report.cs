using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.PortalEngine
        };

        public override ReportResults GetResults()
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
                Data = identicalPageLayouts.AsResult(Metadata.Terms.IdenticalPageLayouts)
            };

            if (countIdenticalPageLayouts == 0)
            {
                results.Summary = Metadata.Terms.NoIdenticalPageLayoutsFound;
            }
            else
            {
                results.Summary = Metadata.Terms.CountIdenticalPageLayoutFound.With(new { count = countIdenticalPageLayouts });
            }

            return results;
        }
    }
}