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

namespace KenticoInspector.Reports.RobotsConfigurationSummary
{
    public class Report : IReport
    {
        private readonly IDatabaseService _databaseService;
        private readonly IInstanceService _instanceService;
        private HttpClient _httpClient = new HttpClient();

        public Report(IDatabaseService databaseService, IInstanceService instanceService, HttpClient httpClient = null)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
            if (httpClient != null)
            {
                _httpClient = httpClient;
            }
        }

        public string Codename => "robots-txt-information-summary";

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"<p>See <a href=""http://www.robotstxt.org/robotstxt.html"" target=""_blank"">robotstxt.org</a> for more details.</p>";

        public string Name => "Robots.txt Configuration Summary";

        public string ShortDescription => "Checks that the ~/robots.txt file is present and accessible.";

        public IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.SEO,
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);

            var instanceUri = new Uri(instance.Url);
            var testUri = new Uri(instanceUri,Constants.RobotsTxtRelativePath);

            var found = ConfirmUriStatusCode(testUri, HttpStatusCode.OK).Result;

            return new ReportResults
            {
                Data = string.Empty,
                Status = found ? ReportResultsStatus.Good : ReportResultsStatus.Warning,
                Summary = found ? "robots.txt found" : "robots.txt not found",
                Type = ReportResultsType.String
            };
        }

        private async Task<bool> ConfirmUriStatusCode(Uri testUri, HttpStatusCode expectedStatusCode)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(testUri);
            return response.StatusCode == expectedStatusCode;
        }
    }
}