using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KenticoInspector.Reports.UnusedPageTypeSummary
{
    public class Report : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public string Codename => "unused-page-type-summary";

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => "This report checks for page types that are not in use.";

        public string Name => "Unused Page Type Summary";

        public string ShortDescription => "Checks for unused pages types.";

        public IList<string> Tags => new List<string>
        {
            ReportTags.Information
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);
            var unusedPageTypes = new List<PageType>(_databaseService.ExecuteSqlFromFile<PageType>(Scripts.GetUnusedPageTypes));

            return new ReportResults
            {
                Type = ReportResultsType.Table,
                Status = ReportResultsStatus.Information,
                Summary = $"{unusedPageTypes.Count} unused page type" + (unusedPageTypes.Count != 1 ? "s." : "."),
                Data = new TableResult<PageType>()
                {
                    Name = "Unused page types",
                    Rows = unusedPageTypes
                }
            };
        }
    }
}
