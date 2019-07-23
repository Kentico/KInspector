using Newtonsoft.Json;

namespace KenticoInspector.Core.Models
{
    public class ReportMetadata<T> where T: new()
    {
        public string Name { get; set; }

        public Descriptions Descriptions { get; set; }

        [JsonIgnore]
        public T Terms { get; set; }
    }

    public class Descriptions
    {
        public string Short { get; set; }

        public string Long { get; set; }
    }
}