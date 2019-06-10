using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Linq;

namespace KenticoInspector.Reports.WebPartSecurityAnalysis
{
    public class Report : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;
        public string LikePageTemplateDisplayName { get; set; } = "%";

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public string Codename => "web-part-security-analysis";

        public IList<Version> CompatibleVersions => new List<Version>()
        {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0"),
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => "Analyzes possible vulnerabilities in Portal Engine web parts. Not applicable to MVC development model.";

        public string Name => "Web Part Security Analysis";

        public string ShortDescription => "";

        public IList<string> Tags => new List<string>()
        {
            ReportTags.PortalEngine,
            ReportTags.Security
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

            List<string> report = new List<string>();
            List<PageTemplate> webPartsInTransformationsTable = GetPageTemplateWebParts(LikePageTemplateDisplayName).ToList<PageTemplate>();
            List<string> whereOrderResults = new List<string>();
            List<string> whereOrderCustomMacroResults = new List<string>();
            List<string> otherResults = new List<string>();
            List<string> otherCustomMacroResults = new List<string>();

            foreach (PageTemplate webPart in webPartsInTransformationsTable)
            {
                string pageTemplateDisplayName = webPart.PageTemplateDisplayName;
                XmlDocument webPartsXmlDoc = new XmlDocument();
                webPartsXmlDoc.LoadXml(webPart.PageTemplateWebParts);

                whereOrderResults.AddRange(AnalyseWhereAndOrderByConditionsInPageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName));
                otherResults.AddRange(AnalysePageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName));

                if (MacroValidator.Current.CheckForCustomMacros(instanceDetails.AdministrationVersion))
                {
                    whereOrderCustomMacroResults.AddRange(AnalyseWhereAndOrderByConditionsInPageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName, MacroValidator.MacroType.Custom));
                    otherCustomMacroResults.AddRange(AnalysePageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName, MacroValidator.MacroType.Custom));
                }
            }

            if (whereOrderResults.Count > 0)
            {
                report.Add("------------------------ Web parts - Where and Order condition results - Potential SQL injections -----------------");
                report.AddRange(whereOrderResults);
                report.Add("<br /><br />");
            }

            if (whereOrderCustomMacroResults.Count > 0)
            {
                report.Add("------------------------ Web parts - Where and Order condition results - Using deprecated Custom Macro type -----------------");
                report.AddRange(whereOrderCustomMacroResults);
                report.Add("<br /><br />");
            }


            if (otherResults.Count > 0)
            {
                report.Add("------------------------ Macros in DB - Potential XSS -----------------");
                report.AddRange(otherResults);
                report.Add("<br /><br />");
            }

            if (otherCustomMacroResults.Count > 0)
            {
                report.Add("------------------------ Macros in DB - Using deprecated Custom Macro type -----------------");
                report.AddRange(otherCustomMacroResults);
                report.Add("<br /><br />");
            }

            if (report.Count == 0)
            {
                return new ReportResults
                {
                    Summary = "No problems in web parts found.",
                    Status = ReportResultsStatus.Good
                };
            }

            StringBuilder res = new StringBuilder();
            report.ForEach(it => res.Append(it.Replace("\n", "<br />")));

            return new ReportResults
            {
                Data = report,
                Summary = "Web part is trusted",
                Type = ReportResultsType.StringList

            };
        }

        #region "Methods"

        private IEnumerable<PageTemplate> GetPageTemplateWebParts(string likePageTemplateDisplayName)
        {
            // A generic of the expected type is needed for this method to run, return a custom type
            var result = _databaseService.ExecuteSqlFromFile<PageTemplate>(Scripts.GetPageTemplateWebParts,
                new { PageTemplateDisplayName = likePageTemplateDisplayName });

            return result;
        }

        /// <summary>
        /// Analyses page template web parts. Searches for <c>where</c> and <c>order</c> properites
        /// of web parts, which have their value defined using macros of the type(s) defined in <paramref name="macroTypes"/>.
        /// </summary>
        /// <param name="pageTemplateWebParts">Page template web parts XML.</param>
        /// <param name="pageTemplateName">Page template name.</param>
        /// <param name="macroTypes">The macro types for search.</param>
        /// <returns>Enumeration of analysis results.</returns>
        private List<string> AnalyseWhereAndOrderByConditionsInPageTemplateWebParts(XmlDocument pageTemplateWebParts, string pageTemplateName, MacroValidator.MacroType macroTypes = MacroValidator.MacroType.All)
        {
            List<string> res = new List<string>();

            foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
            {
                foreach (XmlNode propertyNode in webPartNode.SelectNodes("property"))
                {
                    XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                    string innerText = propertyNode.InnerText;
                    if ((nameAttribute != null) && (nameAttribute.Value.Contains("where") || nameAttribute.Value.Contains("order")) && (!string.IsNullOrEmpty(innerText) && (!innerText.Contains("ToInt"))))
                    {
                        bool containsMacros = MacroValidator.Current.ContainsMacros(innerText, macroTypes);
                        if (containsMacros)
                        {
                            string report =
                                $"Web part: {webPartNode.Attributes["controlid"].Value}/{webPartNode.Attributes["type"].Value}, property: {nameAttribute.Value} <br /> <strong>{MacroValidator.Current.HighlightMacros(HttpUtility.HtmlEncode(innerText), macroTypes)}</strong>.<br />";

                            res.Add(report);
                        }
                    }
                }
            }
            // The any method is not accessible for some reason now
            if (res.Count != 0)
            {
                res.Insert(0, "<strong>Page template name: </strong>" + pageTemplateName);
            }

            return res;
        }

        /// <summary>
        /// Analyses page template web parts. Searches for all properties of web parts except
        /// <c>where</c> and <c>order</c>, which have their value defined using macros of the type(s) defined in <paramref name="macroTypes"/>.
        /// </summary>
        /// <param name="pageTemplateWebParts">Page template web parts XML.</param>
        /// <param name="pageTemplateName">Page template name.</param>
        /// <param name="macroTypes">The macro types for search.</param>
        /// <returns>Enumeration of analysis results.</returns>
        private List<string> AnalysePageTemplateWebParts(XmlDocument pageTemplateWebParts, string pageTemplateName, MacroValidator.MacroType macroTypes = MacroValidator.MacroType.All)
        {
            List<string> res = new List<string>();
            foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
            {
                foreach (XmlNode propertyNode in webPartNode.SelectNodes("property"))
                {
                    XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                    string innerText = propertyNode.InnerText;
                    if ((nameAttribute != null) && (nameAttribute.Value.Contains("text") || nameAttribute.Value.Contains("content")) && (!string.IsNullOrEmpty(innerText) && !innerText.Contains("|(encode)")))
                    {
                        bool containsMacros = MacroValidator.Current.ContainsMacros(innerText, macroTypes);
                        if (containsMacros)
                        {
                            string report = string.Format("Web part: {0}/{1}, property: {2} <br /> <strong>{3}</strong>.<br />",
                                    webPartNode.Attributes["controlid"].Value,
                                    webPartNode.Attributes["type"].Value,
                                    nameAttribute.Value,
                                    MacroValidator.Current.HighlightMacros(HttpUtility.HtmlEncode(innerText)), macroTypes);

                            res.Add(report);
                        }
                    }
                }
            }

            if (res.Count != 0)
            {
                res.Insert(0, "<strong>Page template name: </strong>" + pageTemplateName);
            }

            return res;
        }

        #endregion
    }
}
