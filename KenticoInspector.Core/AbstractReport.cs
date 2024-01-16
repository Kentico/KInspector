using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Core
{
    public abstract class AbstractReport<T> : AbstractModule<T>, IReport where T : new()
    {
        protected AbstractReport(IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
        }

        public abstract ReportResults GetResults();
    }
}