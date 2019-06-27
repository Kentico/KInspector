using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public IReport GetReport(string codename)
        {
            return _reportRepository.GetReport(codename);
        }

        public ReportResults GetReportResults(string reportCodename, Guid instanceGuid)
        {
            var report = _reportRepository.GetReport(reportCodename);
            return report.GetResults(instanceGuid);
        }

        public IEnumerable<IReport> GetReports(ReportFilter reportFilter = null)
        {
            return _reportRepository.GetReports(reportFilter);
        }
    }
}