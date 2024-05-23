using KInspector.Core.Models;

namespace KInspector.Core.Modules
{
    /// <summary>
    /// A module which modifies data in the connected instance.
    /// </summary>
    public interface IAction : IModule
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="optionsJson">A serialized object containing parameters to execute the action with.</param>
        Task<ModuleResults> Execute(string optionsJson);

        /// <summary>
        /// Gets the type of the object expected when executing the action.
        /// </summary>
        Type GetOptionsType();
    }
}