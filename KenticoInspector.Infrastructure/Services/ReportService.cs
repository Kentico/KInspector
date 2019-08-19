using System;
using System.Collections.Generic;

using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IInstanceService instanceService;
        private readonly IDatabaseService databaseService;

        public ReportService(IReportRepository reportRepository, IInstanceService instanceService, IDatabaseService databaseService)
        {
            this.reportRepository = reportRepository;
            this.instanceService = instanceService;
            this.databaseService = databaseService;
        }

        public IReport GetReport(string codename)
        {
            return reportRepository.GetReport(codename);
        }

        public ReportResults GetReportResults(string reportCodename, Guid instanceGuid)
        {
            var report = reportRepository.GetReport(reportCodename);
            var instance = instanceService.SetCurrentInstance(instanceGuid);

            databaseService.Configure(instance.DatabaseSettings);

            return report.GetResults();
        }

        public IEnumerable<IReport> GetReports(Guid instanceGuid, ReportFilter reportFilter = null)
        {
            instanceService.SetCurrentInstance(instanceGuid);

            return reportRepository.GetReports(reportFilter);
        }
    }
}