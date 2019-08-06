using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.PageTypeNotAssignedToSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.PageTypeNotAssignedToSite
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
            var pageTypesNotAssigned = databaseService.ExecuteSqlFromFile<UnassignedPageTypes>(Scripts.PageTypeNotAssigned);

            return CompileResults(pageTypesNotAssigned);
        }

        private ReportResults CompileResults(IEnumerable<UnassignedPageTypes> unassignedPageTypes)
        {
            var countUnassignedPageTypes = unassignedPageTypes.Count();

            var results = new ReportResults
            {
                Status = ReportResultsStatus.Information,
                Type = ReportResultsType.Table,
                Data = new TableResult<dynamic>()
                {
                    Name = Metadata.Terms.PageTypesNotAssigned,
                    Rows = unassignedPageTypes
                }
            };

            if (countUnassignedPageTypes == 0)
            {
                results.Summary = Metadata.Terms.AllpageTypesAssigned;
            }
            else
            {
                results.Summary = Metadata.Terms.CountPageTypeNotAssigned.With(new { count = countUnassignedPageTypes });
            }

            return results;
        }
    }
}