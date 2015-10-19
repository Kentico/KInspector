using System;
using System.Collections.Generic;

namespace Kentico.KInspector.Core
{
    /// <summary>
    /// A struct that is to be returned for each module. Needs to be created only in <see cref="IModule.GetModuleMetadata"/>.
    /// </summary>
    public struct ModuleMetadata
    {
        /// <summary>
        /// Name of the of the module. For example 'EventLog information' or 'Online marketing database size'.
        /// </summary>
        public string Name;

        /// <summary>
        /// Detailed information on what does the module do.
        /// </summary>
        /// <example>
        /// Shows detailed info about Online marketing table sizes and throws warning if some of them is too big.
        /// </example>
        public string Comment;

        /// <summary>
        /// All versions that the module supports. 
        /// </summary>
        public ICollection<Version> SupportedVersions;

        /// <summary>
        /// Optional marker for category of the module. Can be for example 'Online marketing', 'Security', 'Content'. If left empty, category 'General' will be used.
        /// </summary>
        /// <remarks>
        /// Module whose category starts with <c>Setup</c> is considered an instance setup module. Such module may perform changes in the database.
        /// </remarks>
        public string Category;
    }
}
