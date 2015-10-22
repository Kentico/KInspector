namespace Kentico.KInspector.Core
{
    /// <summary>
    /// The interface to implement to add a functionality to this application. 
    /// Add DLL with implementation of this interface to the same folder as executing assembly to auto-load
    /// all of them.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Returns the metadata of the module.
        /// </summary>
        /// <example>
        /// <code>
        /// public ModuleMetadata GetModuleMetadata()
        /// {
        ///     return new ModuleMetadata()
        ///     {
        ///         Name = "EventLog Information",
        ///         SupportedVersions = new[] { new Version("6.0"), new Version("7.0") },
        ///         Comment = "Checks event log for information like 404 pages and logged exceptions."
        ///     };
        /// }
        /// </code>
        /// </example>
        ModuleMetadata GetModuleMetadata();


        /// <summary>
        /// Returns the whole result set of the module.
        /// </summary>
        /// <example>
        /// <code> 
        /// public ModuleResults GetResults(InstanceInfo instanceInfo)
        /// {
        ///    var dbService = instanceInfo.DBService;
        ///    var results = dbService.ExecuteAndGetDataSetFromFile("EventLogInfoModule.sql");
        ///
        ///    return new ModuleResults()
        ///    {
        ///        Result = results,
        ///        ResultComment = "Check event log for more details!"
        ///    };
        /// }
        /// </code> 
        /// </example>
        ModuleResults GetResults(InstanceInfo instanceInfo);
    }
}
