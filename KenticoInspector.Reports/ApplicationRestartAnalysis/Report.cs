﻿using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models.Data;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.EventLog,
            ReportTags.Health
        };

        public Report(
            IDatabaseService databaseService,
            IModuleMetadataService moduleMetadataService
            ) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ReportResults GetResults()
        {
            var cmsEventLogs = databaseService.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode);

            return CompileResults(cmsEventLogs);
        }

        private ReportResults CompileResults(IEnumerable<CmsEventLog> cmsEventLogs)
        {
            if (!cmsEventLogs.Any())
            {
                return new ReportResults
                {
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var totalEvents = cmsEventLogs.Count();
            var totalStartEvents = cmsEventLogs.Count(e => e.EventCode == "STARTAPP");
            var totalEndEvents = cmsEventLogs.Count(e => e.EventCode == "ENDAPP");
            var earliestTime = totalEvents > 0
                ? cmsEventLogs.Min(e => e.EventTime)
                : new DateTime();

            var latestTime = totalEvents > 0
                ? cmsEventLogs.Max(e => e.EventTime)
                : new DateTime();

            var summary = Metadata.Terms.Summaries.Information.With(new
            {
                earliestTime,
                latestTime,
                totalEndEvents,
                totalEvents,
                totalStartEvents
            });

            var data = new TableResult<CmsEventLog>()
            {
                Name = Metadata.Terms.TableTitles.ApplicationRestartEvents,
                Rows = cmsEventLogs
            };

            return new ReportResults
            {
                Data = data,
                Status = ResultsStatus.Information,
                Summary = summary,
                Type = ResultsType.Table
            };
        }
    }
}