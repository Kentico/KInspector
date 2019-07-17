using System;
using System.Collections.Generic;
using System.Data;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseConsistencyCheck.Models;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck
{
    public class Report : IReport, IWithMetadata<Labels>
    {
        private readonly IDatabaseService databaseService;
        private readonly ILabelService labelService;

        public Report(IDatabaseService databaseService, ILabelService labelService)
        {
            this.databaseService = databaseService;
            this.labelService = labelService;
        }

        public string Codename => nameof(DatabaseConsistencyCheck);

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version(10, 0),
            new Version(11, 0),
            new Version(12, 0)
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public IList<string> Tags => new List<string> {
            ReportTags.Health
        };

        public Metadata<Labels> Metadata => labelService.GetMetadata<Labels>(Codename);

        public ReportResults GetResults()
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
                    Summary = Metadata.Labels.CheckResultsTableForAnyIssues,
                    Data = checkDbResults
                };
            }
            else
            {
                return new ReportResults
                {
                    Type = ReportResultsType.String,
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Labels.NoIssuesFound,
                    Data = string.Empty
                };
            }
        }
    }
}