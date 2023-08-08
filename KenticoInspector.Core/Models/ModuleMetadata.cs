using Newtonsoft.Json;

namespace KenticoInspector.Core.Models
{
    public class ModuleMetadata<T> where T : new()
    {
        public ModuleDetails Details { get; set; } = new ModuleDetails();

        [JsonIgnore]
        public T Terms { get; set; } = new T();
    }
}
