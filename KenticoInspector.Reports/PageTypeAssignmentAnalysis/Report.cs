using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis
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
            ReportTags.Health,
            ReportTags.Consistency
        };

        public override ReportResults GetResults()
        {
            var unassignedPageTypes = databaseService.ExecuteSqlFromFile<PageType>(Scripts.GetPageTypesNotAssignedToSite);

            return CompileResults(unassignedPageTypes);
        }

        private ReportResults CompileResults(IEnumerable<PageType> unassignedPageTypes)
        {
            var results = new ReportResults
            {
                Status = ReportResultsStatus.Good,
                Summary = Metadata.Terms.NoIssuesFound,
                Data = new TableResult<PageType>
                {
                    Name = Metadata.Terms.UnassignedPageTypesTableHeader,
                    Rows = unassignedPageTypes
                }
            };

            var unassignedPageTypeCount = unassignedPageTypes.Count();
            if (unassignedPageTypeCount > 0)
            {
                results.Status = ReportResultsStatus.Warning;
                results.Summary = Metadata.Terms.WarningSummary.With(new { unassignedPageTypeCount });
            }

            return results;
        }
    }
}