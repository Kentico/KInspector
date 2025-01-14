using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    /// <summary>
    /// Contains methods for managing the KInspector configuration file.
    /// </summary>
    public interface IConfigService : IService
    {
        /// <summary>
        /// Removes an instance from the configuration file.
        /// </summary>
        /// <param name="guid">The GUID of the instance to remove.</param>
        /// <returns><c>True</c> if the operation was successful.</returns>
        bool DeleteInstance(Guid? guid);

        /// <summary>
        /// Gets an instance from the configuration file.
        /// </summary>
        /// <param name="guid">The GUID of the instance to retrieve.</param>
        Instance? GetInstance(Guid guid);

        /// <summary>
        /// Gets the contents of the configuration file.
        /// </summary>
        InspectorConfig GetConfig();

        /// <summary>
        /// Adds an instance to the configuration file.
        /// </summary>
        void UpsertInstance(Instance instance);

        /// <summary>
        /// Gets the currently connected instance from the configuration file, or <c>null</c> if none is set.
        /// </summary>
        Instance? GetCurrentInstance();

        /// <summary>
        /// Saves the currently connected instance to the configuration file.
        /// </summary>
        /// <returns>The instance with the provided <paramref name="guid"/>, or <c>null</c> if not found.</returns>
        Instance? SetCurrentInstance(Guid? guid);
    }
}