using KenticoInspector.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Models
{
    public class InstanceDetails
    {
        public Guid Guid { get; set; }

        [JsonConverter(typeof(VersionObjectConverter))]
        public Version AdministrationVersion { get; set; }

        [JsonConverter(typeof(VersionObjectConverter))]
        public Version DatabaseVersion { get; set; }

        public IEnumerable<Site> Sites { get; set; }
    }
}