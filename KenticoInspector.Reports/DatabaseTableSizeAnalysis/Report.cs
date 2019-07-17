using System;
using System.Collections.Generic;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis
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

        public string Codename => nameof(DatabaseTableSizeAnalysis);

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
            var top25LargestTables = databaseService.ExecuteSqlFromFile<DatabaseTableSizeResult>(Scripts.GetTop25LargestTables);

            return new ReportResults
            {
                Type = ReportResultsType.Table,
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Labels.CheckResultsTableForAnyIssues,
                Data = new TableResult<DatabaseTableSizeResult>()
                {
                    Name = Metadata.Labels.Top25Results,
                    Rows = top25LargestTables
                }
            };
        }
    }
}