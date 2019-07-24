using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IReportMetadataService : IService
    {
        string CurrentCultureName { get; }

        ReportMetadata<TTerms> GetReportMetadata<TTerms>(string reportCodename) where TTerms : new();
    }
}