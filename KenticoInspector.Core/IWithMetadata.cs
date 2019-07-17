using KenticoInspector.Core.Models;

namespace KenticoInspector.Core
{
    public interface IWithMetadata<TLabels> where TLabels : new()
    {
        Metadata<TLabels> Metadata { get; }
    }
}