using System;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis
{
    public class AppSettingsAnalyzers
    {
        private Terms ReportTerms { get; }

        public AppSettingsAnalyzers(Terms reportTerms)
        {
            ReportTerms = reportTerms;
        }

        public WebConfigSettingResult CMSRenewSessionAuthChange(WebConfigSetting webConfigSetting)
            => UseStringAnalysis(webConfigSetting, "true", ReportTerms.RecommendationReasons.CMSRenewSessionAuthChange);

        public WebConfigSettingResult CMSHashStringSalt(WebConfigSetting webConfigSetting)
            => UseFuncAnalysis(
                webConfigSetting,
                v => !string.IsNullOrEmpty(v),
                ReportTerms.RecommendedValues.NotEmpty,
                ReportTerms.RecommendationReasons.CMSHashStringSalt
                );

        private WebConfigSettingResult UseStringAnalysis(
            WebConfigSetting webConfigSetting,
            string recommendedValue,
            Term recommendationReason
            )
        {
            var valueIsRecommended = webConfigSetting.KeyValue
                .Equals(recommendedValue, StringComparison.InvariantCultureIgnoreCase);

            if (valueIsRecommended) return null;

            return new WebConfigSettingResult(webConfigSetting, recommendedValue, recommendationReason);
        }

        private WebConfigSettingResult UseFuncAnalysis(
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