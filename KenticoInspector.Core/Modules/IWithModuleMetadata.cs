using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Modules
{
    public interface IWithModuleMetadata<T> where T : new()
    {
        ModuleMetadata<T> Metadata { get; }
    }
}