namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents the local configuration of the KInspector application.
    /// </summary>
    public class InspectorConfig
    {
        /// <summary>
        /// The GUID of the currently connected instance.
        /// </summary>
        public Guid? CurrentInstance { get; set; }

        /// <summary>
        /// A list of registered Kentico instances.
        /// </summary>
        public List<Instance> Instances { get; set; } = new();
    }
}
