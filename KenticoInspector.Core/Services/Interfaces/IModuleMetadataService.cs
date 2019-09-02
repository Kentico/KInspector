using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IModuleMetadataService : IService
    {
        string DefaultCultureName { get; }

        string CurrentCultureName { get; }

        ModuleMetadata<T> GetModuleMetadata<T>(string moduleCodename) where T : new();
    }
}