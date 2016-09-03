namespace Kentico.KInspector.Modules.Export
{
    /// <summary>
    /// A struct containing export module metadata. Used as property of <see cref="IExportModule"/>.
    /// </summary>
    public class ExportModuleMetaData
    {
        /// <summary>
        /// Metadata for an export module.
        /// </summary>
        /// <param name="moduleDisplayName">User-friendly name of the module. Property <see cref="ModuleDisplayName"/>.</param>
        /// <param name="moduleCodeName">Unique name of the module. Property <see cref="ModuleCodeName"/>.</param>
        /// <param name="moduleFileExtension">Extension to be used with the result stream. Property <see cref="ModuleFileExtension"/>.</param>
        /// <param name="moduleFileMimeType">MimeType of the result stream. Property <see cref="ModuleFileMimeType"/>.</param>
        public ExportModuleMetaData(string moduleDisplayName, string moduleCodeName, string moduleFileExtension, string moduleFileMimeType)
        {
            ModuleDisplayName = moduleDisplayName;
            ModuleCodeName = moduleCodeName;
            ModuleFileExtension = moduleFileExtension;
            ModuleFileMimeType = moduleFileMimeType;
        }

        /// <summary>
        /// User-friendly name of the module.
        /// </summary>
        public string ModuleDisplayName { get; }

        /// <summary>
        /// Unique name of the module.
        /// </summary>
        public string ModuleCodeName { get; }

        /// <summary>
        /// Extension to be used with the result stream.
        /// </summary>
        public string ModuleFileExtension { get; }

        /// <summary>
        /// MimeType of the result stream.
        /// </summary>
        public string ModuleFileMimeType { get; }
    }
}
