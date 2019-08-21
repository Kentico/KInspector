using System;
using System.Xml.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class ConnectionStringsAnalyzers : AbstractAnalyzers<WebConfigSetting, WebConfigSettingResult>
    {
        public ConnectionStringsAnalyzers(Terms reportTerms) : base(reportTerms)
        {
        }

        public WebConfigSettingResult CMSConnectionString(XElement connectionStringElement)
            => AnalyzeUsingFunc(
                new WebConfigSetting(
                    connectionStringElement,
                    "CMSConnectionString",
                    connectionStringElement.Attribute("connectionString")?.Value
                    ),
                value => !value.Contains("user id=sa;", StringComparison.InvariantCultureIgnoreCase),
                ReportTerms.RecommendedValues.NotSaUser,
                ReportTerms.RecommendationReasons.SystemWebSettings.CompilationDebug
                );

        protected override WebConfigSettingResult AnalyzeUsingFunc(
            WebConfigSetting systemWebSetting,
            Func<string, bool> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            if (valueIsRecommended(systemWebSetting.KeyValue)) return null;

            return new WebConfigSettingResult(systemWebSetting, recommendedValue, recommendationReason);
        }
    }
}