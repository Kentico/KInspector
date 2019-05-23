using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KenticoInspector.Reports.UnusedPageTypeSummary
{
    class Report : IReport
    {
        public string Codename => "";

        public IList<Version> CompatibleVersions => new List<Version>();

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => "";

        public string Name => "";

        public string ShortDescription => "";

        public IList<string> Tags => new List<string>();

        public ReportResults GetResults(Guid InstanceGuid)
        {
            return new ReportResults();
        }
    }
}
