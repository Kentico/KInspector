using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IReportService : IService
    {
        IReport GetReport(string codename);

        ReportResults GetReportResults(string reportCodename, Guid instanceGuid);

        IEnumerable<IReport> GetReports();
    }
}