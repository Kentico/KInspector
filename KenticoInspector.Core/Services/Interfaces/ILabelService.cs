using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface ILabelService : IService
    {
        string CurrentCultureName { get; }

        Metadata<TLabels> GetMetadata<TLabels>(string codename) where TLabels : new();
    }
}