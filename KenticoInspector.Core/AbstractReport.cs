using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Core
{
    public abstract class AbstractReport<T> : IReport, IWithMetadata<T> where T : new()
    {
        protected readonly IReportMetadataService reportMetadataService;
        public AbstractReport(IReportMetadataService reportMetadataService)
        {
            this.reportMetadataService = reportMetadataService;
        }

        public string Codename => GetCodename(this.GetType());

        public static string GetCodename(Type reportType) {
            return GetDirectParentNamespace(reportType);
        }

        public abstract IList<Version> CompatibleVersions { get; }

        public virtual IList<Version> IncompatibleVersions => new List<Version>();

        public abstract IList<string> Tags { get; }

        public ReportMetadata<T> Metadata => reportMetadataService.GetReportMetadata<T>(Codename);

        public abstract ReportResults GetResults();

        private static string GetDirectParentNamespace(Type reportType)
        {
            var fullNameSpace = reportType.Namespace;
            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;
            return fullNameSpace.Substring(indexAfterLastPeriod, fullNameSpace.Length - indexAfterLastPeriod);
        }
    }
}
