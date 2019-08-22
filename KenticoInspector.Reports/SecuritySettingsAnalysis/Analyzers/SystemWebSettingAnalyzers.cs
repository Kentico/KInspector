using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public IEnumerable<((string, string), Func<XElement, WebConfigSettingResult>)> TestAnalyzers2 => new List<((string, string), Func<XElement, WebConfigSettingResult>)>
        {
           (("Authentication", "cookieless"), element => AnalyzeUsingExpression(
                element.Element("forms"),
                value => IsNullOrEquals(value, "useCookies"),
                ReportTerms.RecommendedValues.UseCookies,
                ReportTerms.RecommendationReasons.SystemWebSettings.AuthenticationCookieless
                ))
        };

        public IEnumerable<Expression<Func<XElement, WebConfigSettingResult>>> TestAnalyzers1 => new List<Expression<Func<XElement, WebConfigSettingResult>>>
        {
            Authentication => AnalyzeUsingExpression(
                Authentication.Element("forms"),
                cookieless => IsNullOrEquals(cookieless, "useCookies"),
                ReportTerms.RecommendedValues.UseCookies,
                ReportTerms.RecommendationReasons.SystemWebSettings.AuthenticationCookieless
                )
        };

        public bool TestMatch1(Expression<Func<XElement, WebConfigSettingResult>> analyzer, string name)
        {
            return analyzer.Parameters[0].Name
                .Equals(name, StringComparison.InvariantCulture);
        }

        public WebConfigSettingResult Authentication(XElement systemWebElement)
            => AnalyzeUsingFunc(
                new WebConfigSetting(
                    systemWebElement.Element("forms"),
                    "cookieless",
                    systemWebElement.Element("forms").Attribute("cookieless")
                    ),
                value => IsNullOrEquals(value, "useCookies"),
                ReportTerms.RecommendedValues.UseCookies,
                ReportTerms.RecommendationReasons.SystemWebSettings.AuthenticationCookieless
                );

        public WebConfigSettingResult Compilation(XElement systemWebElement)
            => AnalyzeUsingString(
                new WebConfigSetting(systemWebElement, "debug", systemWebElement.Attribute("debug")),
                "false",
                ReportTerms.RecommendationReasons.SystemWebSettings.CompilationDebug
                );

        public WebConfigSettingResult CustomErrors(XElement systemWebElement)
            => AnalyzeUsingFunc(
                new WebConfigSetting(systemWebElement, "mode", systemWebElement.Attribute("mode")),
                value => IsNullOrEquals(value, "on"),
                ReportTerms.RecommendedValues.NotOn,
                ReportTerms.RecommendationReasons.SystemWebSettings.CustomErrorsMode
                );

        public WebConfigSettingResult HttpCookies(XElement systemWebElement)
            => AnalyzeUsingString(
                new WebConfigSetting(systemWebElement, "httpOnlyCookies", systemWebElement.Attribute("httpOnlyCookies")),
                "true",
                ReportTerms.RecommendationReasons.SystemWebSettings.HttpCookiesHttpOnlyCookies
                );

        public WebConfigSettingResult Pages(XElement systemWebElement)
            => AnalyzeUsingFunc(
                new WebConfigSetting(systemWebElement, "enableViewState", systemWebElement.Attribute("enableViewState")),
                value => IsNullOrEquals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SystemWebSettings.PagesEnableViewState
                );

        public WebConfigSettingResult Trace(XElement systemWebElement)
            => AnalyzeUsingString(
                new WebConfigSetting(systemWebElement, "enabled", systemWebElement.Attribute("enabled")),
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

        protected WebConfigSettingResult AnalyzeUsingExpression(
            XElement element,
            Expression<Func<string, bool>> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            string attributeName = valueIsRecommended.Parameters[0].Name;

            string keyValue = element.Attribute(attributeName)?.Value;

            if (keyValue != null && valueIsRecommended.Compile()(keyValue)) return null;

            return new WebConfigSettingResult(element, attributeName, keyValue, recommendedValue, recommendationReason);
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