using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Models
{
    public class ConnectedInstanceDetails
    {
        public Guid Guid { get; set; }
        public Version AdministrationVersion { get; set; }
        public Version DatabaseVersion { get; set; }
        public List<Site> Sites { get; set; }
    }
}
