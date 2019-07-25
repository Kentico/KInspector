using KenticoInspector.Core.Models;
using System.Collections.Generic;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<IReport> GetReports(ReportFilter filterSettings = null);

        IReport GetReport(string codename);
    }
}