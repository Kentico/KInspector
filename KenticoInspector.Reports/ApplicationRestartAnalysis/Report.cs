using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
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

        public string Codename => "application-restarts";

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => "<p>Frequent restarts may that there are issues to resolve</p>";

        public string Name => "Application Restart Event Analysis";

        public string ShortDescription => "Shows application restart events from event log";

        public IList<string> Tags => new List<string> {
            ReportTags.EventLog,
            ReportTags.Health
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

            var applicationRestartEvents = _databaseService.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.GetApplicationRestartEvents);

            return CompileResults(applicationRestartEvents);
        }

        private static ReportResults CompileResults(IEnumerable<ApplicationRestartEvent> applicationRestartEvents)
        {
            var data = new TableResult<dynamic>()
            {
                Name = "Application Restart Events",
                Rows = applicationRestartEvents
            };

            var totalEvents = applicationRestartEvents.Count();
            var totalStartEvents = applicationRestartEvents.Where(e => e.EventCode == "STARTAPP").Count();
            var totalEndEvents = applicationRestartEvents.Where(e => e.EventCode == "ENDAPP").Count();
            var earliestTime = totalEvents > 0 ? applicationRestartEvents.Min(e => e.EventTime) : new DateTime();
            var latestTime = totalEvents > 0 ? applicationRestartEvents.Max(e => e.EventTime) : new DateTime();

            var totalEventsText = $"{totalEvents} event {(totalEvents == 1 ? string.Empty : "s")}";
            var totalStartEventsText = $"{totalStartEvents} start{(totalStartEvents == 1 ? string.Empty : "s")}";
            var totalEndEventsText = $"{totalEndEvents} end{(totalEndEvents == 1 ? string.Empty : "s")}";
            var timeSpanText = earliestTime.Year > 1 ? $"spanning {earliestTime.ToString()} - {latestTime.ToString()}" : string.Empty;

            var results = new ReportResults
            {
                Type = ReportResultsType.Table,
                Status = ReportResultsStatus.Information,
                Summary = $"{totalEventsText} ({totalStartEventsText}, {totalEndEventsText}) {timeSpanText}",
                Data = data
            };

            return results;
        }
    }
}
