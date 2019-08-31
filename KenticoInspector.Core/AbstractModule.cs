using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Core
{
    public abstract class AbstractModule<T> : IModule, IWithModuleMetadata<T> where T : new()
    {
        protected readonly IModuleMetadataService moduleMetadataService;

        private ModuleMetadata<T> metadata;

        public AbstractModule(IModuleMetadataService moduleMetadataService)
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
                return metadata ?? (metadata = moduleMetadataService.GetModuleMetadata<T>(Codename));
            }
        }

        public abstract IList<string> Tags { get; }

        public static string GetCodename(Type reportType)
        {
            return GetDirectParentNamespace(reportType);
        }

        private static string GetDirectParentNamespace(Type reportType)
        {
            var fullNameSpace = reportType.Namespace;
            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;
            return fullNameSpace.Substring(indexAfterLastPeriod, fullNameSpace.Length - indexAfterLastPeriod);
        }
    }
}