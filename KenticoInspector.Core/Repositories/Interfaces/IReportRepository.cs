using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<IReport> GetReports();

        IReport GetReport(string codename);
    }
}