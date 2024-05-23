using KInspector.Core.Models;

namespace KInspector.Core.Modules
{
    /// <summary>
    /// A module that contains metadata.
    /// </summary>
    public interface IWithModuleMetadata<T> where T : new()
    {
        /// <summary>
        /// The module metadata.
        /// </summary>
        ModuleMetadata<T> Metadata { get; }
    }
}