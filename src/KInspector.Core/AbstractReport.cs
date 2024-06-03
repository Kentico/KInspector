using KInspector.Core.Models;
using KInspector.Core.Modules;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Core
{
    public abstract class AbstractReport<T> : AbstractModule<T>, IReport where T : new()
    {
        protected AbstractReport(IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
        }

        public abstract Task<ModuleResults> GetResults();
    }
}