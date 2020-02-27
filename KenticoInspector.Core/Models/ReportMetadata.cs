using Newtonsoft.Json;

namespace KenticoInspector.Core.Models
{
    public class ReportMetadata<T> where T : new()
    {
        public ReportDetails Details { get; set; } = new ReportDetails();

        [JsonIgnore]
        public T Terms { get; set; } = new T();
    }
}
