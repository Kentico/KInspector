using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models;

namespace KenticoInspector.Reports.UnusedPageTypeSummary
{
    public class UnusedPageTypeSummaryReport : IReport
    {
        private readonly IDatabaseService databaseService;
        private readonly ILabelService labelService;

        public UnusedPageTypeSummaryReport(IDatabaseService databaseService, ILabelService labelService)
        {
            this.databaseService = databaseService;
            this.labelService = labelService;
        }

        public string Codename => nameof(UnusedPageTypeSummary);

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version(10, 0),
            new Version(11, 0),
            new Version(12, 0)
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public IList<string> Tags => new List<string>
        {
            ReportTags.Information
        };

        public Metadata<Labels> Metadata => labelService.GetMetadata<Labels>(Codename);

        public ReportResults GetResults()
        {
            var unusedPageTypes = databaseService.ExecuteSqlFromFile<PageType>(Scripts.GetUnusedPageTypes);

            var countOfUnusedPageTypes = unusedPageTypes.Count();

            return new ReportResults
            {
                Type = ReportResultsType.Table,
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Labels.CountUnusedPageType.With(new { count = countOfUnusedPageTypes }),
                Data = new TableResult<PageType>()
                {
                    Name = Metadata.Labels.UnusedPageTypes,
                    Rows = unusedPageTypes
                }
            };
        }
    }
}