using KInspector.Core.Models;
using KInspector.Core.Modules;

namespace KInspector.Core.Services.Interfaces
{
    /// <summary>
    /// Contains methods for getting modules from repositories and executing them.
    /// </summary>
    public interface IModuleService : IService
    {
        /// <summary>
        /// Gets the report with the provided codename, or <c>null</c> if it doesn't exist.
        /// </summary>
        IReport? GetReport(string codename);

        /// <summary>
        /// Executes a report.
        /// </summary>
        /// <param name="report">The report to execute.</param>
        /// <param name="callback">The function called when the results are returned.</param>
        Task GetReportResults(IReport report, Action<ModuleResults> callback);

        /// <summary>
        /// Gets a list of filtered reports.
        /// </summary>
        /// <param name="getUntested">If <c>true</c>, reports where the Kentico major version is not present in either <see cref="IModule.CompatibleVersions"/>
        /// or <see cref="IModule.IncompatibleVersions"/> are included.</param>
        /// <param name="getIncompatible">If <c>true</c>, reports where the Kentico major version is present in <see cref="IModule.IncompatibleVersions"/>
        /// are included.</param>
        /// <param name="tag">If provided, only reports with this tag are included.</param>
        /// <param name="name">If provided, only reports where <see cref="ModuleDetails.Name"/> contains the string are included.</param>
        IEnumerable<IReport> GetReports(bool getUntested = false, bool getIncompatible = false, string? tag = null, string? name = null);

        /// <summary>
        /// Gets a list of filtered actions.
        /// </summary>
        /// <param name="getUntested">If <c>true</c>, actions where the Kentico major version is not present in either <see cref="IModule.CompatibleVersions"/>
        /// or <see cref="IModule.IncompatibleVersions"/> are included.</param>
        /// <param name="getIncompatible">If <c>true</c>, actions where the Kentico major version is present in <see cref="IModule.IncompatibleVersions"/>
        /// are included.</param>
        /// <param name="tag">If provided, only actions with this tag are included.</param>
        /// <param name="name">If provided, only actions where <see cref="ModuleDetails.Name"/> contains the string are included.</param>
        IEnumerable<IAction> GetActions(bool getUntested = false, bool getIncompatible = false, string? tag = null, string? name = null);

        /// <summary>
        /// Gets the action with the provided codename, or <c>null</c> if it doesn't exist.
        /// </summary>
        IAction? GetAction(string codename);

        /// <summary>
        /// Executes an action.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="optionsJson">A serialized object containing parameters to execute the action with.</param>
        /// <param name="callback">The function called when the results are returned.</param>
        Task ExecuteAction(IAction action, string optionsJson, Action<ModuleResults> callback);
    }
}