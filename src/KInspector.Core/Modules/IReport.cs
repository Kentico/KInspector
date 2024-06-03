using KInspector.Core.Models;

namespace KInspector.Core.Modules
{
    /// <summary>
    /// A module which returns information from the connected instance without modifying data.
    /// </summary>
    public interface IReport : IModule
    {
        /// <summary>
        /// Executes the report.
        /// </summary>
        Task<ModuleResults> GetResults();
    }
}