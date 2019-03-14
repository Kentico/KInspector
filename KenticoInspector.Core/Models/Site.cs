using System;

namespace KenticoInspector.Core.Models
{ 
    public class Site
    {
        public bool ContentOnly { get; set; }
        public string DomainName { get; set; }
        public Guid GUID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string PresentationUrl { get; set; }
    }
}