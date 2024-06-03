using KInspector.Core.Modules;
using KInspector.Core.Repositories.Interfaces;

namespace KInspector.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IEnumerable<IReport> reports;

        public ReportRepository(IEnumerable<IReport> reports)
        {
            this.reports = reports;
        }

        public IReport? GetReport(string codename) =>
            reports.FirstOrDefault(x => x.Codename.Equals(codename, StringComparison.InvariantCultureIgnoreCase));

        public IEnumerable<IReport> GetReports() => reports;
    }
}