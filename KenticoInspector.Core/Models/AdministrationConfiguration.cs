
using Newtonsoft.Json;
using System;
using System.IO;

namespace KenticoInspector.Core.Models
{
    public class AdministrationConfiguration
    {
        public Uri Uri { get; set; }

        public string DirectoryPath { get; set; }

        [JsonIgnore]
        public DirectoryInfo DirectoryInfo { get; set; }

        public AdministrationConfiguration() { }

        public AdministrationConfiguration(string url, string path)
        {
            Uri = new Uri(url);
            DirectoryPath = path;
            DirectoryInfo = new DirectoryInfo(path);
        }
    }
}