namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents a Kentico instance and its configuration.
    /// </summary>
    public class Instance
    {
        /// <summary>
        /// The configuration required to connect to the instance's database.
        /// </summary>
        public DatabaseSettings DatabaseSettings { get; set; } = new();

        /// <summary>
        /// The instance GUID.
        /// </summary>
        public Guid? Guid { get; set; }

        /// <summary>
        /// The instance name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The absolute path to the administration website root directory.
        /// </summary>
        public string? AdministrationPath { get; set; }

        /// <summary>
        /// The administration website's URL.
        /// </summary>
        public string? AdministrationUrl { get; set; }
    }
}