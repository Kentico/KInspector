using KenticoInspector.Core.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Models
{
    public class ActionResult
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsStatus Status { get; set; }

        public string Summary { get; set; }
    }
}
