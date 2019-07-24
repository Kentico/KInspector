using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System.IO;
using System.Threading;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KenticoInspector.Core.Helpers
{
    public class ReportMetadataService : IReportMetadataService
    {
        public string CurrentCultureName => Thread.CurrentThread.CurrentCulture.Name;

        public ReportMetadata<T> GetReportMetadata<T>(string reportCodename) where T : new()
        {
            var yamlPath = $"{DirectoryHelper.GetExecutingDirectory()}\\{reportCodename}\\Metadata\\{CurrentCultureName}.yaml";
            return DeserializeYaml<ReportMetadata<T>>(yamlPath);
        }

        public T DeserializeYaml<T>(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            var yamlFile = File.ReadAllText(path);
            return deserializer.Deserialize<T>(yamlFile);
        }
    }
}