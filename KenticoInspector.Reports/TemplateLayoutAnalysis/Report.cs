using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis
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

        public string Codename => "TemplateLayoutAnalysis";

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => "Returns the list of templates with identical custom layouts (whitespace sensitive).";

        public string Name => "Templates with identical layouts";

        public string ShortDescription => "Returns a list of identical page layouts.";

        public IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.PortalEngine
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

            var identicalLayouts = _databaseService.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts);

            return CompileResults(identicalLayouts);
        }

        private static ReportResults CompileResults(IEnumerable<IdenticalPageLayouts> identicalPageLayouts)
        {
            var layoutResults = new TableResult<dynamic>()
            {
                Name = "Identical page layouts",
                Rows = identicalPageLayouts
            };

            var results = new ReportResults
            {
                Status = ReportResultsStatus.Information
            };
            

            if (layoutResults.Rows.Count() == 0)
            {
                results.Summary = "No Identical page layouts found.";
                results.Data = layoutResults;
                results.Type = ReportResultsType.Table;
            }
            else
            {
                results.Summary = "Identical page layouts found.";
                results.Data = layoutResults;
                results.Type = ReportResultsType.Table;
            }

            return results;
        }
    }
}
