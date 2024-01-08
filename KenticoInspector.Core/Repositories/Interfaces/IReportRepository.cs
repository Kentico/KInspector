using KenticoInspector.Core.Modules;

using System.Collections.Generic;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<IReport> GetReports();

        IReport GetReport(string codename);
    }
}