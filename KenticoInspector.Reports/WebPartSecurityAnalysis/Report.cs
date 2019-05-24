using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace KenticoInspector.Reports.WebPartSecurityAnalysis
{
    public class Report : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public string Codename => "web-part-security-analysis";

        public IList<Version> CompatibleVersions => new List<Version>()
        {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0"),
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => "Analyzes possible vulnerabilities in Portal Engine web parts. Not applicable to MVC development model.";

        public string Name => "Web Part Security Analysis";

        public string ShortDescription => "";

        public IList<string> Tags => new List<string>()
        {
            ReportTags.PortalEngine,
            ReportTags.Security
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

            return new ReportResults();
        }
    }
}
