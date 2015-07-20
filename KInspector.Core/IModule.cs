namespace KInspector.Core
{
    /// <summary>
    /// The interface to implement to add a functionality to this application. 
    /// Add DLL with implementation of this interface to the same folder as executing assembly to auto-load
    /// all of them.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Returns the metadata of the module
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// public ModuleMetadata GetModuleMetadata()
        /// {
        ///    return new ModuleMetadata() 
        ///    { 
        ///        Name = "EventLog Information", 
        ///        Versions = new List<string>() { "8.0", "8.1" },
        ///        Comment = "Checks event log for information like 404 pages and logged exceptions.",
        ///        ResultType = ModuleResultsType.Table
        ///    };
        /// }
        /// ]]>
        /// </example>
        ModuleMetadata GetModuleMetadata();

        /// <summary>
        /// Returns the whole result set of the module.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        /// {
        ///    var dbService = new DatabaseService(config);
        ///    var results = dbService.ExecuteAndGetPrintsFromFile("EventLogInfoModule.sql");
        ///
        ///    return new ModuleResults()
        ///    {
        ///        Result = results,
        ///        ResultComment = "Check event log for more details!"
        ///    };
        /// }
        /// ]]>
        /// </example>
        ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService);
    }
}
