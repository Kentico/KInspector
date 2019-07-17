using System.IO;

using KenticoInspector.Core.Repositories.Interfaces;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KenticoInspector.Core.Helpers
{
    public class YamlRepository : IYamlRepository
    {
        public IDeserializer Deserializer => new DeserializerBuilder()
                                                .WithNamingConvention(new CamelCaseNamingConvention())
                                                .Build();

        public T Deserialize<T>(string path)
        {
            var yamlFile = File.ReadAllText($"{path}");

            var serialized = Deserializer.Deserialize<T>(yamlFile);

            return serialized;
        }
    }
}