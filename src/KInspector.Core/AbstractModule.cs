using KInspector.Core.Models;
using KInspector.Core.Modules;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Core
{
    public abstract class AbstractModule<T> : IModule, IWithModuleMetadata<T> where T : new()
    {
        protected readonly IModuleMetadataService moduleMetadataService;

        private ModuleMetadata<T>? metadata;

        protected AbstractModule(IModuleMetadataService moduleMetadataService)
        {
            this.moduleMetadataService = moduleMetadataService;
        }

        public string Codename => GetCodename(this.GetType());

        public abstract IList<Version> CompatibleVersions { get; }

        public virtual IList<Version> IncompatibleVersions => new List<Version>();

        public ModuleMetadata<T> Metadata
        {
            get
            {
                return metadata ??= moduleMetadataService.GetModuleMetadata<T>(Codename);
            }
        }

        public abstract IList<string> Tags { get; }

        public static string GetCodename(Type reportType)
        {
            return GetDirectParentNamespace(reportType);
        }

        private static string GetDirectParentNamespace(Type reportType)
        {
            var fullNameSpace = reportType.Namespace ?? throw new InvalidOperationException("Error getting report namespace.");
            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;

            return fullNameSpace.Substring(indexAfterLastPeriod, fullNameSpace.Length - indexAfterLastPeriod);
        }
    }
}