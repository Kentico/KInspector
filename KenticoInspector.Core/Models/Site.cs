﻿using System;

namespace KenticoInspector.Core.Models
{ 
    public class Site
    {
        public Guid InstanceGuid { get; set; }
        public bool ContentOnly { get; set; }
        public string DomainName { get; set; }
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string PresentationUrl { get; set; }
    }
}