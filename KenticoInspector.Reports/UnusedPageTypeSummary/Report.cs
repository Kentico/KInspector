using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.UnusedPageTypeSummary
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
            ReportTags.Information
        };

        public override ReportResults GetResults()
        {
            var unusedPageTypes = databaseService.ExecuteSqlFromFile<PageType>(Scripts.GetUnusedPageTypes);

            var countOfUnusedPageTypes = unusedPageTypes.Count();

            return new ReportResults
            {
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Terms.CountUnusedPageType.With(new { count = countOfUnusedPageTypes }),
                Data = unusedPageTypes.AsResult().WithLabel(Metadata.Terms.UnusedPageTypes)
            };
        }
    }
}