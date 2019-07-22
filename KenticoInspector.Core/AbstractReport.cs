using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Core
{
    public abstract class AbstractReport : IReport
    {
        public string Codename => GetCodename(this.GetType());

        public static string GetCodename(Type reportType) {
            return GetDirectParentNamespace(reportType);
        }

        public abstract IList<Version> CompatibleVersions { get; }

        public virtual IList<Version> IncompatibleVersions => new List<Version>();

        public abstract IList<string> Tags { get; }

        public abstract ReportResults GetResults();

        private static string GetDirectParentNamespace(Type reportType)
        {
            var fullNameSpace = reportType.Namespace;
            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;
            return fullNameSpace.Substring(indexAfterLastPeriod, fullNameSpace.Length - indexAfterLastPeriod);
        }
    }
}
