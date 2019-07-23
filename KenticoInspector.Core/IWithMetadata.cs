using KenticoInspector.Core.Models;

namespace KenticoInspector.Core
{
    public interface IWithMetadata<T> where T : new()
    {
        ReportMetadata<T> Metadata { get; }
    }
}