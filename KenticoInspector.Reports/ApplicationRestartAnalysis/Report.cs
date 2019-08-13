using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string> {
            ReportTags.EventLog,
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var applicationRestartEvents = databaseService.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.ApplicationRestartEvents);

            return CompileResults(applicationRestartEvents);
        }

        private ReportResults CompileResults(IEnumerable<ApplicationRestartEvent> applicationRestartEvents)
        {
            var data = new TableResult<ApplicationRestartEvent>()
            {
                Name = Metadata.Terms.ApplicationRestartEvents,
                Rows = applicationRestartEvents
            };

            var totalEvents = applicationRestartEvents.Count();
            var totalStartEvents = applicationRestartEvents.Where(e => e.EventCode == "STARTAPP").Count();
            var totalEndEvents = applicationRestartEvents.Where(e => e.EventCode == "ENDAPP").Count();
            var earliestTime = totalEvents > 0 ? applicationRestartEvents.Min(e => e.EventTime) : new DateTime();
            var latestTime = totalEvents > 0 ? applicationRestartEvents.Max(e => e.EventTime) : new DateTime();

            var totalEventsText = Metadata.Terms.CountTotalEvent.With(new { count = totalEvents });

            var totalStartEventsText = Metadata.Terms.CountStartEvent.With(new { count = totalStartEvents });

            var totalEndEventsText = Metadata.Terms.CountEndEvent.With(new { count = totalEndEvents });

            string timeSpanText = string.Empty;

            if (earliestTime.Year > 1)
            {
                timeSpanText = Metadata.Terms.SpanningEarliestLatest.With(new { earliestTime, latestTime });
            }

            var results = new ReportResults
            {
                Status = ReportResultsStatus.Information,
                Summary = $"{totalEventsText} ({totalStartEventsText}, {totalEndEventsText}) {timeSpanText}",
                Data = data
            };

            return results;
        }
    }
}