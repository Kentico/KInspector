using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KenticoInspector.Reports.RobotsConfigurationSummary
{
    public class Report : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;
        private HttpClient _httpClient = new HttpClient();

        public Report(IDatabaseService databaseService, IInstanceService instanceService, HttpClient httpClient = null)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
            if (httpClient != null) _httpClient = httpClient;
        }
        public string Codename => "Robots.txt";

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"Checks that the ~/robots.txt file is present and accessible. See http://www.robotstxt.org/robotstxt.html for more details";

        public string Name => "Robots.txt";

        public string ShortDescription => "Checks that the ~/robots.txt file is present and accessible.";

        public IList<string> Tags => new List<string>
        {
            ReportTags.Database,
            ReportTags.Health,
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            string filePath = "/robots.txt";
            Uri siteDomain = new Uri(instance.Url);
            var result = TestUrl(siteDomain, filePath).Result;

            if (!result)
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Warning,
                    Type = ReportResultsType.String,
                    Summary = "robots.txt not found",
                };
            }
            else
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Type = ReportResultsType.String,
                    Summary = "robots.txt found",
                };
            }
        }

        private async Task<bool> TestUrl(Uri url, string file)
        {
            Uri requestUrl = new Uri(url, file);
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
