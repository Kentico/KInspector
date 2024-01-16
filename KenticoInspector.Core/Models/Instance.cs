using System;

namespace KenticoInspector.Core.Models
{
    public class Instance
    {
        public DatabaseSettings DatabaseSettings { get; set; }

        public Guid Guid { get; set; }

        public string Name { get; set; }

        public string AdminPath { get; set; }

        public string AdminUrl { get; set; }
    }
}