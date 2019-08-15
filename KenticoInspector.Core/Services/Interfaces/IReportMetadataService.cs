using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IReportMetadataService : IService
    {
        string DefaultCultureName { get; }

        string CurrentCultureName { get; }

        ReportMetadata<T> GetReportMetadata<T>(string reportCodename) where T : new();
    }
}