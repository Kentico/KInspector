using Newtonsoft.Json;

namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents basic details about the module and all terms used in the module's function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModuleMetadata<T> where T : new()
    {
        /// <summary>
        /// The basic details about the module.
        /// </summary>
        public ModuleDetails Details { get; set; } = new ModuleDetails();

        /// <summary>
        /// Terms to be by the module results.
        /// </summary>
        [JsonIgnore]
        public T Terms { get; set; } = new T();
    }
}
