using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using KenticoInspector.Core.Constants;

using System.Dynamic;

namespace KenticoInspector.Core.Models
{
    public class ActionResults
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsStatus Status { get; set; }

        public string Summary { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsType Type { get; set; }

        public dynamic Data { get; set; }

        public ActionResults()
        {
            Data = new ExpandoObject();
        }
    }
}
