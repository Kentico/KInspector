using YamlDotNet.Serialization;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IYamlRepository : IRepository
    {
        IDeserializer Deserializer { get; }

        T Deserialize<T>(string path);
    }
}