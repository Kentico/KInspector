using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
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

        public string Codename => nameof(ApplicationRestartAnalysis);

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version(10, 0),
            new Version(11, 0)
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public IList<string> Tags => new List<string> {
            ReportTags.EventLog,
            ReportTags.Health
        };

        public Metadata<Labels> Metadata => labelService.GetMetadata<Labels>(Codename);

        public ReportResults GetResults()
        {
            var applicationRestartEvents = databaseService.ExecuteSqlFromFile<ApplicationRestartEvent>(Scripts.ApplicationRestartEvents);

            return CompileResults(applicationRestartEvents);
        }

        private ReportResults CompileResults(IEnumerable<ApplicationRestartEvent> applicationRestartEvents)
        {
            var data = new TableResult<dynamic>()
            {
                Name = Metadata.Labels.ApplicationRestartEvents,
                Rows = applicationRestartEvents
            };

            var totalEvents = applicationRestartEvents.Count();
            var totalStartEvents = applicationRestartEvents.Where(e => e.EventCode == "STARTAPP").Count();
            var totalEndEvents = applicationRestartEvents.Where(e => e.EventCode == "ENDAPP").Count();
            var earliestTime = totalEvents > 0 ? applicationRestartEvents.Min(e => e.EventTime) : new DateTime();
            var latestTime = totalEvents > 0 ? applicationRestartEvents.Max(e => e.EventTime) : new DateTime();

            var totalEventsText = Metadata.Labels.CountTotalEvent.With(new { count = totalEvents });

            var totalStartEventsText = Metadata.Labels.CountStartEvent.With(new { count = totalStartEvents });

            var totalEndEventsText = Metadata.Labels.CountEndEvent.With(new { count = totalEndEvents });

            string timeSpanText = string.Empty;

            if (earliestTime.Year > 1)
            {
                timeSpanText = Metadata.Labels.SpanningEarliestLatest.With(new { earliestTime, latestTime });
            }

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