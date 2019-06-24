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

            throw new NotImplementedException();
        }
    }
}
