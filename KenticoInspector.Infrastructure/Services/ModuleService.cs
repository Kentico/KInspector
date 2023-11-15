using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Infrastructure.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly IReportRepository reportRepository;
        private readonly IActionRepository actionRepository;

        public ModuleService(IReportRepository reportRepository, IActionRepository actionRepository, IInstanceService instanceService, IDatabaseService databaseService)
        {
            this.reportRepository = reportRepository;
            this.actionRepository = actionRepository;
            this.instanceService = instanceService;
            this.databaseService = databaseService;
        }

        public ActionResults ExecuteAction(string codename, Guid instanceGuid, string optionsJson)
        {
            var action = actionRepository.GetAction(codename);
            var instance = instanceService.SetCurrentInstance(instanceGuid);
            databaseService.Configure(instance.DatabaseSettings);

            return action.Execute(optionsJson);
        }

        public IAction GetAction(string codename) => actionRepository.GetAction(codename);

        public IEnumerable<IAction> GetActions(Guid instanceGuid)
        {
            instanceService.SetCurrentInstance(instanceGuid);
            return actionRepository.GetActions();
        }

        public IReport GetReport(string codename) => reportRepository.GetReport(codename);

        public ReportResults GetReportResults(string codename, Guid instanceGuid)
        {
            var report = reportRepository.GetReport(codename);
            var instance = instanceService.SetCurrentInstance(instanceGuid);

            databaseService.Configure(instance.DatabaseSettings);

            return report.GetResults();
        }

        public IEnumerable<IReport> GetReports(Guid instanceGuid)
        {
            instanceService.SetCurrentInstance(instanceGuid);
            return reportRepository.GetReports();
        }
    }
}