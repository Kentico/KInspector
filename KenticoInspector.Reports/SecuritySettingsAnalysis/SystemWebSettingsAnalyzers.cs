using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis
{
    public class SystemWebSettingsAnalyzers
    {
        private Terms ReportTerms { get; }

        public SystemWebSettingsAnalyzers(Terms reportTerms)
        {
            ReportTerms = reportTerms;
        }

        public WebConfigSettingResult Compilation(XElement systemWebElement)
            => UseStringAnalysis(
                systemWebElement,
                element => "debug",
                element => element.Attribute("debug").Value,
                ReportTerms.RecommendationReasons.CompilationDebug,
                "false");

        public WebConfigSettingResult Trace(XElement systemWebElement)
            => UseStringAnalysis(
                systemWebElement,
                element => "enabled",
                element => element.Attribute("enabled")?.Value,
                ReportTerms.RecommendationReasons.TraceEnabled,
                "false");

        private WebConfigSettingResult UseStringAnalysis(
            XElement systemWebElement,
            Func<XElement, string> getSettingName,
            Func<XElement, string> getSettingValue,
            Term recommendationReason,
            string recommendedValue,
            [CallerMemberName] string elementName = null
            )
        {
            var elementNameMatches = systemWebElement.Name.ToString()
                .Equals(elementName, StringComparison.InvariantCultureIgnoreCase);

            if (!elementNameMatches) return null;

            var settingValue = getSettingValue(systemWebElement);

            var valueIsRecommended = settingValue
                .Equals(recommendedValue, StringComparison.InvariantCultureIgnoreCase);

            if (valueIsRecommended) return null;

            var systemWebSetting = new WebConfigSetting(systemWebElement, getSettingName(systemWebElement), settingValue);

            return new WebConfigSettingResult(systemWebSetting, recommendedValue, recommendationReason);
        }
    }
}