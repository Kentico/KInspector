using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseConsistencyCheck.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            var checkDbResults = databaseService.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults);
#pragma warning restore 0618

            return CompileResults(checkDbResults);
        }

        private ReportResults CompileResults(DataTable checkDbResults)
        {
            var hasIssues = checkDbResults.Rows.Count > 0;

            if (hasIssues)
            {
                return new ReportResults
                {
                    Type = ReportResultsType.Table,
                    Status = ReportResultsStatus.Error,
                    Summary = Metadata.Terms.CheckResultsTableForAnyIssues,
                    Data = checkDbResults
                };
            }
            else
            {
                return new ReportResults
                {
                    Type = ReportResultsType.String,
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.NoIssuesFound,
                    Data = string.Empty
                };
            }
        }
    }
}