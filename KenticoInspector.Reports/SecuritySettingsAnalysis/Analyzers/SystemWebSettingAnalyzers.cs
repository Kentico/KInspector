using System;
using System.Xml.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class SystemWebSettingAnalyzers : AbstractAnalyzers<WebConfigSetting, WebConfigSettingResult>
    {
        public SystemWebSettingAnalyzers(Terms reportTerms) : base(reportTerms)
        {
        }

        public WebConfigSettingResult Authentication(XElement systemWebElement)
            => AnalyzeUsingFunc(
                new WebConfigSetting(
                    systemWebElement.Element("forms"),
                    "cookieless",
                    systemWebElement.Element("forms").Attribute("cookieless")?.Value
                    ),
                value => IsNullOrEquals(value, "true"),
                ReportTerms.RecommendedValues.UseCookies,
                ReportTerms.RecommendationReasons.SystemWebSettings.AuthenticationCookieless
                );

        public WebConfigSettingResult Compilation(XElement systemWebElement)
            => AnalyzeUsingString(
                new WebConfigSetting(systemWebElement, "debug", systemWebElement.Attribute("debug")?.Value),
                "false",
                ReportTerms.RecommendationReasons.SystemWebSettings.CompilationDebug
                );

        public WebConfigSettingResult CustomErrors(XElement systemWebElement)
            => AnalyzeUsingFunc(
                new WebConfigSetting(systemWebElement, "mode", systemWebElement.Attribute("mode")?.Value),
                value => IsNullOrEquals(value, "on"),
                ReportTerms.RecommendedValues.NotOn,
                ReportTerms.RecommendationReasons.SystemWebSettings.CustomErrorsMode
                );

        public WebConfigSettingResult HttpCookies(XElement systemWebElement)
            => AnalyzeUsingString(
                new WebConfigSetting(systemWebElement, "httpOnlyCookies", systemWebElement.Attribute("httpOnlyCookies")?.Value),
                "true",
                ReportTerms.RecommendationReasons.SystemWebSettings.HttpCookiesHttpOnlyCookies
                );

        public WebConfigSettingResult Pages(XElement systemWebElement)
            => AnalyzeUsingFunc(
                new WebConfigSetting(systemWebElement, "enableViewState", systemWebElement.Attribute("enableViewState")?.Value),
                value => IsNullOrEquals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SystemWebSettings.PagesEnableViewState
                );

        public WebConfigSettingResult Trace(XElement systemWebElement)
            => AnalyzeUsingString(
                new WebConfigSetting(systemWebElement, "enabled", systemWebElement.Attribute("enabled")?.Value),
                "false",
                ReportTerms.RecommendationReasons.SystemWebSettings.TraceEnabled
                );

        private static bool IsNullOrEquals(string value, string equals)
        {
            return value == null || value.Equals(equals, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override bool Match(string analyzerName, string name)
        {
            return analyzerName
                .Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

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