using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class AppSettingAnalyzers : AbstractAnalyzers<XElement, WebConfigSettingResult>
    {
        public override IEnumerable<Expression<Func<XElement, WebConfigSettingResult>>> Analyzers
            => new List<Expression<Func<XElement, WebConfigSettingResult>>>
        {
            CMSEnableCsrfProtection => AnalyzeUsingExpression(
                CMSEnableCsrfProtection,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.AppSettings.CMSEnableCsrfProtection
                ),
            CMSHashStringSalt => AnalyzeUsingExpression(
                CMSHashStringSalt,
                value => !string.IsNullOrEmpty(value),
                ReportTerms.RecommendedValues.NotEmpty,
                ReportTerms.RecommendationReasons.AppSettings.CMSHashStringSalt
                ),
            CMSRenewSessionAuthChange => AnalyzeUsingExpression(
                CMSRenewSessionAuthChange,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.AppSettings.CMSRenewSessionAuthChange
                ),
             CMSXFrameOptionsExcluded => AnalyzeUsingExpression(
                CMSXFrameOptionsExcluded,
                value => string.IsNullOrEmpty(value),
                ReportTerms.RecommendedValues.Empty,
                ReportTerms.RecommendationReasons.AppSettings.CMSXFrameOptionsExcluded
                )
        };

        public AppSettingAnalyzers(Terms reportTerms) : base(reportTerms)
        {
        }

        protected override WebConfigSettingResult AnalyzeUsingExpression(
            XElement appSetting,
            Expression<Func<string, bool>> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            string attributeName = valueIsRecommended.Parameters[0].Name;

            string keyValue = appSetting.Attribute(attributeName)?.Value;

            if (valueIsRecommended.Compile()(keyValue)) return null;

            string keyName = appSetting.Attribute("key").Value;

            return new WebConfigSettingResult(appSetting, keyName, keyValue, recommendedValue, recommendationReason);
        }
    }
}