using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.RobotsConfigurationSummary.Models;

namespace KenticoInspector.Reports.RobotsConfigurationSummary
{
    public class RobotsConfigurationSummaryReport : IReport, IWithMetadata<Labels>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly ILabelService labelService;
        private HttpClient _httpClient = new HttpClient();

        public RobotsConfigurationSummaryReport(IDatabaseService databaseService, IInstanceService instanceService, ILabelService labelService, HttpClient httpClient = null)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.labelService = labelService;

            if (httpClient != null)
            {
                _httpClient = httpClient;
            }
        }

        public string Codename => nameof(RobotsConfigurationSummary);

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version(10, 0),
            new Version(11, 0),
            new Version(12, 0)
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.SEO,
        };

        public Metadata<Labels> Metadata => labelService.GetMetadata<Labels>(Codename);

        public ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceUri = new Uri(instance.Url);
            var testUri = new Uri(instanceUri, Constants.RobotsTxtRelativePath);

            var found = ConfirmUriStatusCode(testUri, HttpStatusCode.OK).Result;

            return new ReportResults
            {
                Data = string.Empty,
                Status = found ? ReportResultsStatus.Good : ReportResultsStatus.Warning,
                Summary = found ? Metadata.Labels.RobotsTxtFound : Metadata.Labels.RobotsTxtNotFound,
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