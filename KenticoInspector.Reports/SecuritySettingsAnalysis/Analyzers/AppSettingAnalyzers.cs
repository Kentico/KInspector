using System;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class AppSettingAnalyzers : AbstractAnalyzers<WebConfigSetting, WebConfigSettingResult>
    {
        public AppSettingAnalyzers(Terms reportTerms) : base(reportTerms)
        {
        }

        public WebConfigSettingResult CMSEnableCsrfProtection(WebConfigSetting webConfigSetting)
            => AnalyzeUsingString(
                webConfigSetting,
                "true",
                ReportTerms.RecommendationReasons.AppSettings.CMSEnableCsrfProtection
                );

        public WebConfigSettingResult CMSHashStringSalt(WebConfigSetting webConfigSetting)
            => AnalyzeUsingFunc(
                webConfigSetting,
                value => !string.IsNullOrEmpty(value),
                ReportTerms.RecommendedValues.NotEmpty,
                ReportTerms.RecommendationReasons.AppSettings.CMSHashStringSalt
                );

        public WebConfigSettingResult CMSRenewSessionAuthChange(WebConfigSetting webConfigSetting)
            => AnalyzeUsingString(
                webConfigSetting,
                "true",
                ReportTerms.RecommendationReasons.AppSettings.CMSRenewSessionAuthChange
                );

        public WebConfigSettingResult CMSXFrameOptionsExcluded(WebConfigSetting webConfigSetting)
            => AnalyzeUsingFunc(
                webConfigSetting,
                value => string.IsNullOrEmpty(value),
                ReportTerms.RecommendedValues.Empty,
                ReportTerms.RecommendationReasons.AppSettings.CMSXFrameOptionsExcluded
                );

        protected override WebConfigSettingResult AnalyzeUsingFunc(
            WebConfigSetting webConfigSetting,
            Func<string, bool> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            if (valueIsRecommended(webConfigSetting.KeyValue)) return null;

            return new WebConfigSettingResult(webConfigSetting, recommendedValue, recommendationReason);
        }
    }
}