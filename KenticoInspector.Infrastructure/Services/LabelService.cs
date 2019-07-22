using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System.IO;
using System.Threading;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KenticoInspector.Core.Helpers
{
    public class LabelService : ILabelService
    {
        public string CurrentCultureName => Thread.CurrentThread.CurrentCulture.Name;

        public Metadata<TLabels> GetMetadata<TLabels>(string reportCodename) where TLabels : new()
        {
            var yamlPath = $"{DirectoryHelper.GetExecutingDirectory()}\\{reportCodename}\\Metadata\\{CurrentCultureName}.yaml";
            return DeserializeYaml<Metadata<TLabels>>(yamlPath);
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