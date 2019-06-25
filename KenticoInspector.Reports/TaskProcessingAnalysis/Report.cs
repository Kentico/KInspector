using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.TaskProcessingAnalysis
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

        public string Codename => "task-processing-analysis";

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"
        <p>The following queues are reviewed for stuck tasks:</p>
        <ul>
            <li>Scheduled Tasks (ignores recurring)</li>
            <li>Web Farm Tasks</li>
            <li>Integration Bus Tasks</li>
            <li>Staging Tasks</li>
            <li>Search Tasks</li>
        </ul>
        ";

        public string Name => "Task Processing Analysis";

        public string ShortDescription => "Checks system queues for tasks that appear stuck for more than 24 hours.";

        public IList<string> Tags => new List<string> {
           ReportTags.Health
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

            var unprocessedIntegrationBusTasks = _databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedIntegrationBusTasks);
            var unprocessedScheduledTasks = _databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedScheduledTasks);
            var unprocessedSearchTasks = _databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedSearchTasks);
            var unprocessedStagingTasks = _databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedStagingTasks);
            var unprocessedWebFarmTasks = _databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCountOfUnprocessedWebFarmTasks);

            var rawResults = new Dictionary<string, int>();
            rawResults.Add(TaskTypes.IntegrationBusTasks, unprocessedIntegrationBusTasks);
            rawResults.Add(TaskTypes.ScheduledTasks, unprocessedScheduledTasks);
            rawResults.Add(TaskTypes.SearchTasks, unprocessedSearchTasks);
            rawResults.Add(TaskTypes.StagingTasks, unprocessedStagingTasks);
            rawResults.Add(TaskTypes.WebFarmTasks, unprocessedWebFarmTasks);

            return CompileResults(rawResults);
        }

        private ReportResults CompileResults(Dictionary<string, int> taskResults)
        {
            var totalUnprocessedTasks = taskResults.Sum(x => x.Value);
            return new ReportResults()
            {
                Data = taskResults
                    .Where(x => x.Value > 0)
                    .Select(x => $"{x.Value} {x.Key}{(x.Value == 1 ? "" : "s")}"),
                Status = totalUnprocessedTasks > 0 ? ReportResultsStatus.Warning : ReportResultsStatus.Good,
                Summary = $"{totalUnprocessedTasks} unprocessed task{(totalUnprocessedTasks == 1 ? "" : "s")}",
                Type = ReportResultsType.StringList
            };
        }
    }
}
