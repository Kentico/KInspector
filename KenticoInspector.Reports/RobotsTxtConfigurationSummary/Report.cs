using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.RobotsTxtConfigurationSummary.Models;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;

namespace KenticoInspector.Reports.RobotsTxtConfigurationSummary
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IInstanceService instanceService;
        private readonly HttpClient _httpClient = new HttpClient();

        public Report(
            IInstanceService instanceService,
            IModuleMetadataService moduleMetadataService,
            HttpClient httpClient = null
        ) : base(moduleMetadataService)

        {
            this.instanceService = instanceService;

            if (httpClient != null)
            {
                _httpClient = httpClient;
            }
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<Version> IncompatibleVersions => VersionHelper.GetVersionList("13");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.SEO,
        };

        public override ReportResults GetResults()
        {
            var instanceUri = new Uri(instanceService.CurrentInstance.AdminUrl);
            var testUri = new Uri(instanceUri, Constants.RobotsTxtRelativePath);
            var found = ConfirmUriStatusCode(testUri, HttpStatusCode.OK).Result;

            return new ReportResults
            {
                Data = string.Empty,
                Status = found ? ResultsStatus.Good : ResultsStatus.Warning,
                Summary = found ? Metadata.Terms.RobotsTxtFound : Metadata.Terms.RobotsTxtNotFound,
                Type = ResultsType.String
            };
        }

        private async Task<bool> ConfirmUriStatusCode(Uri testUri, HttpStatusCode expectedStatusCode)
        {
            // Ignore invalid certificates
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            HttpResponseMessage response = await _httpClient.GetAsync(testUri);
            return response.StatusCode == expectedStatusCode;
        }
    }
}