using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.RobotsConfigurationSummary.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KenticoInspector.Reports.RobotsConfigurationSummary
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private HttpClient _httpClient = new HttpClient();

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            IReportMetadataService reportMetadataService,
            HttpClient httpClient = null
        ) : base(reportMetadataService)

        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;

            if (httpClient != null)
            {
                _httpClient = httpClient;
            }
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.SEO,
        };

        public override ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceUri = new Uri(instance.Url);
            var testUri = new Uri(instanceUri, Constants.RobotsTxtRelativePath);

            var found = ConfirmUriStatusCode(testUri, HttpStatusCode.OK).Result;

            return new ReportResults
            {
                Data = string.Empty,
                Status = found ? ReportResultsStatus.Good : ReportResultsStatus.Warning,
                Summary = found ? Metadata.Terms.RobotsTxtFound : Metadata.Terms.RobotsTxtNotFound,
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