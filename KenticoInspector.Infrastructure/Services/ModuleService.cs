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

        public ActionResults ExecuteAction<T>(string actionCodename, Guid instanceGuid, T options) where T : new()
        {
            throw new NotImplementedException();
        }

        public IAction GetAction(string codename) => actionRepository.GetAction(codename);

        public IEnumerable<IAction> GetActions(Guid instanceGuid)
        {
            instanceService.SetCurrentInstance(instanceGuid);
            return actionRepository.GetActions();
        }

        public IReport GetReport(string codename) => reportRepository.GetReport(codename);

        public ReportResults GetReportResults(string reportCodename, Guid instanceGuid)
        {
            var report = reportRepository.GetReport(reportCodename);
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