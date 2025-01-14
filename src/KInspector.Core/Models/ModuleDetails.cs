namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents basic details about the module.
    /// </summary>
    public class ModuleDetails
    {
        /// <summary>
        /// The detailed description of the module's function.
        /// </summary>
        public string? LongDescription { get; set; }

        /// <summary>
        /// The human-friendly name of the module.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The short description of the module's function.
        /// </summary>
        public string? ShortDescription { get; set; }
    }
}