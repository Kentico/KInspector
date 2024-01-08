using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
           ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var unprocessedIntegrationBusTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedIntegrationBusTasks);
            var unprocessedScheduledTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedScheduledTasks);
            var unprocessedSearchTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedSearchTasks);
            var unprocessedStagingTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedStagingTasks);
            var unprocessedWebFarmTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedWebFarmTasks);

            var rawResults = new Dictionary<TaskType, int>
            {
                { TaskType.IntegrationBusTask, unprocessedIntegrationBusTasks },
                { TaskType.ScheduledTask, unprocessedScheduledTasks },
                { TaskType.SearchTask, unprocessedSearchTasks },
                { TaskType.StagingTask, unprocessedStagingTasks },
                { TaskType.WebFarmTask, unprocessedWebFarmTasks }
            };

            return CompileResults(rawResults);
        }

        private string AsTaskCountLabel(KeyValuePair<TaskType, int> taskTypeCount)
        {
            Term label = string.Empty;
            var count = taskTypeCount.Value;

            switch (taskTypeCount.Key)
            {
                case TaskType.IntegrationBusTask:
                    label = Metadata.Terms.CountIntegrationBusTask.With(new { count });
                    break;

                case TaskType.ScheduledTask:
                    label = Metadata.Terms.CountScheduledTask.With(new { count });
                    break;

                case TaskType.SearchTask:
                    label = Metadata.Terms.CountSearchTask.With(new { count });
                    break;

                case TaskType.StagingTask:
                    label = Metadata.Terms.CountStagingTask.With(new { count });
                    break;

                case TaskType.WebFarmTask:
                    label = Metadata.Terms.CountWebFarmTask.With(new { count });
                    break;
            }

            return label.With(new { count });
        }

        private ReportResults CompileResults(Dictionary<TaskType, int> taskResults)
        {
            var totalUnprocessedTasks = taskResults.Sum(x => x.Value);
            return new ReportResults()
            {
                Data = taskResults
                    .Where(x => x.Value > 0)
                    .Select(AsTaskCountLabel),
                Status = totalUnprocessedTasks > 0 ? ResultsStatus.Warning : ResultsStatus.Good,
                Summary = Metadata.Terms.CountUnprocessedTask.With(new { count = totalUnprocessedTasks }),
                Type = ResultsType.StringList
            };
        }
    }
}