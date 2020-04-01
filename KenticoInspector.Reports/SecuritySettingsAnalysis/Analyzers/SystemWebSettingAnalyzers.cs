using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class SystemWebSettingAnalyzers : AbstractAnalyzers<XElement, WebConfigSettingResult>
    {
        public override IEnumerable<Expression<Func<XElement, WebConfigSettingResult>>> Analyzers
            => new List<Expression<Func<XElement, WebConfigSettingResult>>>
        {
            Authentication => AnalyzeUsingExpression(
                Authentication.Element("forms"),
                cookieless => IsNullOrEquals(cookieless, "useCookies"),
                "useCookies",
                ReportTerms.RecommendationReasons.SystemWebSettings.AuthenticationCookieless
                ),
            Compilation => AnalyzeUsingExpression(
                Compilation,
                debug => Equals(debug, "false"),
                "false",
                ReportTerms.RecommendationReasons.SystemWebSettings.CompilationDebug
                ),
            CustomErrors => AnalyzeUsingExpression(
                CustomErrors,
                mode => !IsNullOrEquals(mode, "on"),
                ReportTerms.RecommendedValues.NotOn,
                ReportTerms.RecommendationReasons.SystemWebSettings.CustomErrorsMode
                ),
            HttpCookies => AnalyzeUsingExpression(
                HttpCookies,
                httpOnlyCookies => Equals(httpOnlyCookies, "true"),
                "true",
                ReportTerms.RecommendationReasons.SystemWebSettings.HttpCookiesHttpOnlyCookies
                ),
            Pages => AnalyzeUsingExpression(
                Pages,
                enableViewState => IsNullOrEquals(enableViewState, "true"),
                "true",
                ReportTerms.RecommendationReasons.SystemWebSettings.PagesEnableViewState
                ),
            Pages => AnalyzeUsingExpression(
                Pages,
                enableViewStateMac => IsNullOrEquals(enableViewStateMac, "true"),
                "true",
                ReportTerms.RecommendationReasons.SystemWebSettings.PagesEnableViewStateMac
                ),
            Trace => AnalyzeUsingExpression(
                Trace,
                enabled => IsNullOrEquals(enabled, "false"),
                "false",
                ReportTerms.RecommendationReasons.SystemWebSettings.TraceEnabled
                )
        };

        public SystemWebSettingAnalyzers(Terms reportTerms) : base(reportTerms)
        {
        }

        protected override bool Match(string analyzerName, string name)
        {
            return analyzerName
                .Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override WebConfigSettingResult AnalyzeUsingExpression(
            XElement systemWebSetting,
            Expression<Func<string, bool>> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            string attributeName = valueIsRecommended.Parameters[0].Name;

            string keyValue = systemWebSetting.Attribute(attributeName)?.Value;

            if (valueIsRecommended.Compile()(keyValue)) return null;

            return new WebConfigSettingResult(systemWebSetting, attributeName, keyValue, recommendedValue, recommendationReason);
        }
    }
}