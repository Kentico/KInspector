using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    /// <summary>
    /// Contains methods for reading module metadata .yaml files.
    /// </summary>
    public interface IModuleMetadataService : IService
    {
        /// <summary>
        /// The name of the default culture, which corresponds with the name of the .yaml file to load.
        /// </summary>
        string DefaultCultureName { get; }

        /// <summary>
        /// The <see cref="Thread"/>'s current culture, which corresponds with the name of the .yaml file to load.
        /// </summary>
        string CurrentCultureName { get; }

        /// <summary>
        /// Gets the provided module's metadata.
        /// </summary>
        ModuleMetadata<T> GetModuleMetadata<T>(string moduleCodename) where T : new();

        /// <summary>
        /// Gets the provided module's details.
        /// </summary>
        ModuleDetails GetModuleDetails(string moduleCodename);
    }
}