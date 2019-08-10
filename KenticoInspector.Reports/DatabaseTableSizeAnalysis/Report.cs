using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string> {
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var top25LargestTables = databaseService.ExecuteSqlFromFile<DatabaseTableSizeResult>(Scripts.GetTop25LargestTables);

            return new ReportResults
            {
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Terms.CheckResultsTableForAnyIssues,
                Data = {
                    new TableResult<DatabaseTableSizeResult>()
                    {
                        Name = Metadata.Terms.Top25Results,
                        Rows = top25LargestTables
                    }
                }
            };
        }
    }
}