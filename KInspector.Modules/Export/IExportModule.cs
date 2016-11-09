using System.Collections.Generic;
using System.IO;

using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules.Export
{
    /// <summary>
    /// The interface to implement for adding an export format.
    /// Add DLL with implementation of this interface to the same folder as executing assembly to auto-load
    /// all of them.
    /// </summary>
    public interface IExportModule
    {
        /// <summary>
        /// Metadata of the module.
        /// </summary>
        /// <example>
        /// <code>
        /// public ExportModuleMetaData ModuleMetaData => new ExportModuleMetaData("Excel", "ExportXlsx", "xlsx", "application/xlsx");
        /// </code>
        /// </example>
        ExportModuleMetaData ModuleMetaData { get; }

        /// <summary>
        /// Returns stream result of the export process.
        /// </summary>
        /// <param name="moduleNames">Modules to export.</param>
        /// <param name="instanceInfo">Instance for which to execute modules.</param>
        /// <returns></returns>
        Stream GetExportStream(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo);
    }
}
