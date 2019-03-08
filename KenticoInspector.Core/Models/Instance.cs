using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Models
{
    public class Instance
    {
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public Version KenticoDatabaseVersion { get; set; }

        public Version KenticoAdministrationVersion { get; set; }

        public List<Site> Sites { get; set; }
    }
}
