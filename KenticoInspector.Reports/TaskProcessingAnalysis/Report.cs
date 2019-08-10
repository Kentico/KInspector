using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
           ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var unprocessedIntegrationBusTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedIntegrationBusTasks);
            var unprocessedScheduledTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedScheduledTasks);
            var unprocessedSearchTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedSearchTasks);
            var unprocessedStagingTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedStagingTasks);
            var unprocessedWebFarmTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedWebFarmTasks);

            var rawResults = new List<TaskCountResult>
            {
                new TaskCountResult(Metadata.Terms.CountIntegrationBusTask, unprocessedIntegrationBusTasks ),
                new TaskCountResult(Metadata.Terms.CountScheduledTask, unprocessedScheduledTasks ),
                new TaskCountResult(Metadata.Terms.CountSearchTask, unprocessedSearchTasks ),
                new TaskCountResult(Metadata.Terms.CountStagingTask, unprocessedStagingTasks ),
                new TaskCountResult(Metadata.Terms.CountWebFarmTask, unprocessedWebFarmTasks )
            };

            return CompileResults(rawResults);
        }

        private ReportResults CompileResults(IEnumerable<TaskCountResult> taskTypesAndCounts)
        {
            var count = taskTypesAndCounts.Sum(x => x.Count);

            if (count == 0)
            {
                return new ReportResults()
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var data = taskTypesAndCounts
                .Where(taskTypeAndCount => taskTypeAndCount.Count > 0)
                .Select(AsTaskCountLine)
                .ToList();

            return new ReportResults()
            {
                Status = ReportResultsStatus.Warning,
                Summary = Metadata.Terms.CountUnprocessedTask.With(new { count }),
                Data = data
            };
        }

        private static Result AsTaskCountLine(TaskCountResult taskCountResult)
        {
            var count = taskCountResult.Count;

            return taskCountResult.Term.With(new { count });
        }
    }
}