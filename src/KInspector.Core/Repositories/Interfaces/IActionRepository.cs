using KInspector.Core.Modules;

namespace KInspector.Core.Repositories.Interfaces
{
    /// <summary>
    /// Contains all actions found in referenced assemblies.
    /// </summary>
    public interface IActionRepository : IRepository
    {
        /// <summary>
        /// Gets all registered actions.
        /// </summary>
        IEnumerable<IAction> GetActions();

        /// <summary>
        /// Gets the action with the provided codename, or <c>null</c> if not found.
        /// </summary>
        IAction? GetAction(string codename);
    }
}