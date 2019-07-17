using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;

namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public class TaskProcessingAnalysisReport : IReport, IWithMetadata<Labels>
    {
        private readonly IDatabaseService databaseService;
        private readonly ILabelService labelService;

        public TaskProcessingAnalysisReport(IDatabaseService databaseService, ILabelService labelService)
        {
            this.databaseService = databaseService;
            this.labelService = labelService;
        }

        public string Codename => nameof(TaskProcessingAnalysis);

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version(10, 0),
            new Version(11, 0)
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public IList<string> Tags => new List<string> {
           ReportTags.Health
        };

        public Metadata<Labels> Metadata => labelService.GetMetadata<Labels>(Codename);

        public ReportResults GetResults()
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

        private ReportResults CompileResults(Dictionary<TaskType, int> taskResults)
        {
            var totalUnprocessedTasks = taskResults.Sum(x => x.Value);
            return new ReportResults()
            {
                Data = taskResults
                    .Where(x => x.Value > 0)
                    .Select(AsTaskCountLabel),
                Status = totalUnprocessedTasks > 0 ? ReportResultsStatus.Warning : ReportResultsStatus.Good,
                Summary = Metadata.Labels.CountUnprocessedTask.With(new { count = totalUnprocessedTasks }),
                Type = ReportResultsType.StringList
            };
        }

        private string AsTaskCountLabel(KeyValuePair<TaskType, int> taskTypeCount)
        {
            Label label = string.Empty;
            var count = taskTypeCount.Value;

            switch (taskTypeCount.Key)
            {
                case TaskType.IntegrationBusTask:
                    label = Metadata.Labels.CountIntegrationBusTask.With(new { count });
                    break;

                case TaskType.ScheduledTask:
                    label = Metadata.Labels.CountScheduledTask.With(new { count });
                    break;

                case TaskType.SearchTask:
                    label = Metadata.Labels.CountSearchTask.With(new { count });
                    break;

                case TaskType.StagingTask:
                    label = Metadata.Labels.CountStagingTask.With(new { count });
                    break;

                case TaskType.WebFarmTask:
                    label = Metadata.Labels.CountWebFarmTask.With(new { count });
                    break;
            }

            return label.With(new { count });
        }
    }
}