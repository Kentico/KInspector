using KInspector.Core.Modules;

namespace KInspector.Core.Repositories.Interfaces
{
    /// <summary>
    /// Contains all reports found in referenced assemblies.
    /// </summary>
    public interface IReportRepository : IRepository
    {
        /// <summary>
        /// Gets all registered reports.
        /// </summary>
        IEnumerable<IReport> GetReports();

        /// <summary>
        /// Gets the report with the provided codename, or <c>null</c> if not found.
        /// </summary>
        IReport? GetReport(string codename);
    }
}