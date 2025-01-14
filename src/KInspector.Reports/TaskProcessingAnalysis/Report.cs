using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.TaskProcessingAnalysis.Models;

namespace KInspector.Reports.TaskProcessingAnalysis
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
           ModuleTags.Health
        };

        public async override Task<ModuleResults> GetResults()
        {
            var unprocessedIntegrationBusTasks = await databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedIntegrationBusTasks);
            var unprocessedScheduledTasks = await databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedScheduledTasks);
            var unprocessedSearchTasks = await databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedSearchTasks);
            var unprocessedStagingTasks = await databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedStagingTasks);
            var unprocessedWebFarmTasks = await databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedWebFarmTasks);

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
            Term? label = string.Empty;
            var count = taskTypeCount.Value;

            switch (taskTypeCount.Key)
            {
                case TaskType.IntegrationBusTask:
                    label = Metadata.Terms.CountIntegrationBusTask?.With(new { count });
                    break;

                case TaskType.ScheduledTask:
                    label = Metadata.Terms.CountScheduledTask?.With(new { count });
                    break;

                case TaskType.SearchTask:
                    label = Metadata.Terms.CountSearchTask?.With(new { count });
                    break;

                case TaskType.StagingTask:
                    label = Metadata.Terms.CountStagingTask?.With(new { count });
                    break;

                case TaskType.WebFarmTask:
                    label = Metadata.Terms.CountWebFarmTask?.With(new { count });
                    break;
            }

            return label?.With(new { count });
        }

        private ModuleResults CompileResults(Dictionary<TaskType, int> taskResults)
        {
            var totalUnprocessedTasks = taskResults.Sum(x => x.Value);
            var results = new ModuleResults
            {
                Status = totalUnprocessedTasks > 0 ? ResultsStatus.Warning : ResultsStatus.Good,
                Summary = Metadata.Terms.CountUnprocessedTask?.With(new { count = totalUnprocessedTasks }),
                Type = totalUnprocessedTasks > 0 ? ResultsType.StringList : ResultsType.NoResults
            };
            var taskSummaries = taskResults.Where(x => x.Value > 0).Select(AsTaskCountLabel);
            foreach (var str in taskSummaries)
            {
                results.StringResults.Add(str);
            }

            return results;
        }
    }
}