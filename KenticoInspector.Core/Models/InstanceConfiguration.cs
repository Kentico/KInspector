using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Models
{
    public class InstanceConfiguration
    {
        public Guid Guid { get; set; }

        public DatabaseConfiguration DatabaseConfiguration { get; set; }

        public AdministrationConfiguration AdministrationConfiguration { get; set; }

        public InstanceConfiguration() { }

        public InstanceConfiguration(DatabaseConfiguration databaseConfiguration, AdministrationConfiguration administrationConfiguration) {
            Guid = new Guid();
            DatabaseConfiguration = databaseConfiguration;
            AdministrationConfiguration = administrationConfiguration;
        }
    }
}
