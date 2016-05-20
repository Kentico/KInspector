using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class WebPartAnalyzerModule : IModule
    {
        #region "Fields"

        private IDatabaseService mDatabaseService;
        private string mLikePageTemplateDisplayName = "%";

        #endregion


        #region "Properties"

        /// <summary>
        /// Constraints transformation analysis on transformations used in web parts,
        /// which are located on page templates with display name like this one.
        /// </summary>
        /// <remarks>
        /// The current modules implementation does not allow the user to provide
        /// module parameters (unless they are in the global configuration object).
        /// This should be improved to be able to set the value of this property
        /// based on what the user wants.
        /// </remarks>
        public string LikePageTemplateDisplayName
        {
            get
            {
                return mLikePageTemplateDisplayName;
            }
            set
            {
                mLikePageTemplateDisplayName = value;
            }
        }

        #endregion


        #region "IModule interface methods"

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "WebPart analyzer",
                Comment = "Analyzes possible vulnerabilities in web parts.",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Security",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            List<string> report = new List<string>();

            mDatabaseService = instanceInfo.DBService;

            DataTable webPartsInTransformationsTable = GetPageTemplateWebParts(LikePageTemplateDisplayName);
            List<string> whereOrderResults = new List<string>();
            List<string> whereOrderCustomMacroResults = new List<string>();
            List<string> otherResults = new List<string>();
            List<string> otherCustomMacroResults = new List<string>();
            foreach (DataRow webPart in webPartsInTransformationsTable.Rows)
            {
                string pageTemplateDisplayName = webPart["PageTemplateDisplayName"] as string;
                XmlDocument webPartsXmlDoc = new XmlDocument();
                webPartsXmlDoc.LoadXml(webPart["PageTemplateWebParts"] as string);

                whereOrderResults.AddRange(AnalyseWhereAndOrderByConditionsInPageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName));
                whereOrderCustomMacroResults.AddRange(AnalyseWhereAndOrderByConditionsInPageTemplateWebPartsCustomMacros(webPartsXmlDoc, pageTemplateDisplayName, instanceInfo.Version));

                otherResults.AddRange(AnalysePageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName));
                otherCustomMacroResults.AddRange(AnalysePageTemplateWebPartsCustomMacros(webPartsXmlDoc, pageTemplateDisplayName, instanceInfo.Version));
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
                return new ModuleResults
                {
                    ResultComment = "No problems in web parts found.",
                    Status = Status.Good
                };
            }

            StringBuilder res = new StringBuilder();
            report.ForEach(it => res.Append(it.Replace("\n", "<br />")));

            return new ModuleResults
            {
                Result = report,
                Trusted = true
            };
        }


        #endregion


        #region "Methods"

        /// <summary>
        /// Gets page template web parts where page template display name
        /// is like given <paramref name="likePageTemplateDisplayName"/> pattern.
        /// </summary>
        /// <param name="likePageTemplateDisplayName">Like pattern for page template display name.</param>
        /// <returns>DataTable containing page template columns 'PageTemplateDisplayName' and 'PageTemplateWebParts'.</returns>
        private DataTable GetPageTemplateWebParts(string likePageTemplateDisplayName)
        {
            return mDatabaseService.ExecuteAndGetTableFromFile("WebPartAnalyzerModule.sql",
                new SqlParameter("PageTemplateDisplayName", likePageTemplateDisplayName));
        }


        /// <summary>
        /// Analyses page template web parts. Searches for <c>where</c> and <c>order</c> properites
        /// of web parts, which have their value defined using a macro.
        /// </summary>
        /// <param name="pageTemplateWebParts">Page template web parts XML.</param>
        /// <returns>Enumeration of analysis results.</returns>
        private IEnumerable<string> AnalyseWhereAndOrderByConditionsInPageTemplateWebParts(XmlDocument pageTemplateWebParts, string pageTemplateName)
        {
            List<string> res = new List<string>();
            MacroValidator validator = new MacroValidator();
            foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
            {
                foreach (XmlNode propertyNode in webPartNode.SelectNodes("property"))
                {
                    XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                    string innerText = propertyNode.InnerText;
                    if ((nameAttribute != null) && (nameAttribute.Value.Contains("where") || nameAttribute.Value.Contains("order")) && (!string.IsNullOrEmpty(innerText) && (!innerText.Contains("ToInt"))))
                    {
                        bool containsContextOrQueryMacro = validator.ContainsMacros(innerText);
                        if (containsContextOrQueryMacro)
                        {
                            string report = string.Format("Web part: {0}/{1}, property: {2} <br /> <strong>{3}</strong>.<br />",
                                    webPartNode.Attributes["controlid"].Value,
                                    webPartNode.Attributes["type"].Value,
                                    nameAttribute.Value,
                                    validator.HighlightMacros(HttpUtility.HtmlEncode(innerText)));

                            res.Add(report);
                        }
                    }
                }
            }

            if (res.Any())
            {
                res.Insert(0, "<strong>Page template name: </strong>" + pageTemplateName);
            }

            return res;
        }


        private IEnumerable<string> AnalyseWhereAndOrderByConditionsInPageTemplateWebPartsCustomMacros(XmlDocument pageTemplateWebParts, string pageTemplateName, Version version)
        {
            List<string> res = new List<string>();
            MacroValidator validator = new MacroValidator();
            if (validator.CheckForCustomMacros(version))
            {
                foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
                {
                    foreach (XmlNode propertyNode in webPartNode.SelectNodes("property"))
                    {
                        XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                        string innerText = propertyNode.InnerText;
                        if ((nameAttribute != null) && (nameAttribute.Value.Contains("where") || nameAttribute.Value.Contains("order")) && (!string.IsNullOrEmpty(innerText) && (!innerText.Contains("ToInt"))))
                        {
                            bool containsContextOrQueryMacro = validator.ContainsMacros(innerText, MacroValidator.MacroType.Custom);
                            if (containsContextOrQueryMacro)
                            {
                                string report = string.Format("Web part: {0}/{1}, property: {2} <br /> <strong>{3}</strong>.<br />",
                                        webPartNode.Attributes["controlid"].Value,
                                        webPartNode.Attributes["type"].Value,
                                        nameAttribute.Value,
                                        validator.HighlightMacros(HttpUtility.HtmlEncode(innerText), MacroValidator.MacroType.Custom));

                                res.Add(report);
                            }
                        }
                    }
                }
            }

            if (res.Any())
            {
                res.Insert(0, "<strong>Page template name: </strong>" + pageTemplateName);
            }

            return res;

        }

        /// <summary>
        /// Analyses page template web parts. Searches for all properties of web parts except
        /// <c>where</c> and <c>order</c>, which have their value defined using a macro.
        /// </summary>
        /// <param name="pageTemplateWebParts">Page template web parts XML.</param>
        /// <returns>Enumeration of analysis results.</returns>
        private IEnumerable<string> AnalysePageTemplateWebParts(XmlDocument pageTemplateWebParts, string pageTemplateName)
        {
            List<string> res = new List<string>();
            MacroValidator validator = new MacroValidator();

            foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
            {
                foreach (XmlNode propertyNode in webPartNode.SelectNodes("property"))
                {
                    XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                    string innerText = propertyNode.InnerText;
                    if ((nameAttribute != null) && (nameAttribute.Value.Contains("text") || nameAttribute.Value.Contains("content")) && (!string.IsNullOrEmpty(innerText) && !innerText.Contains("|(encode)")))
                    {
                        bool containsContextOrQueryMacro = validator.ContainsMacros(innerText);
                        if (containsContextOrQueryMacro)
                        {
                            string report = string.Format("Web part: {0}/{1}, property: {2} <br /> <strong>{3}</strong>.<br />",
                                    webPartNode.Attributes["controlid"].Value,
                                    webPartNode.Attributes["type"].Value,
                                    nameAttribute.Value,
                                    validator.HighlightMacros(HttpUtility.HtmlEncode(innerText)));

                            res.Add(report);
                        }
                    }
                }
            }

            if (res.Any())
            {
                res.Insert(0, "<strong>Page template name: </strong>" + pageTemplateName);
            }

            return res;
        }


        private IEnumerable<string> AnalysePageTemplateWebPartsCustomMacros(XmlDocument pageTemplateWebParts, string pageTemplateName, Version version)
        {
            List<string> res = new List<string>();
            MacroValidator validator = new MacroValidator();
            if (validator.CheckForCustomMacros(version))
            {
                foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
                {
                    foreach (XmlNode propertyNode in webPartNode.SelectNodes("property"))
                    {
                        XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                        string innerText = propertyNode.InnerText;
                        if ((nameAttribute != null) && (nameAttribute.Value.Contains("text") || nameAttribute.Value.Contains("content")) && (!string.IsNullOrEmpty(innerText) && !innerText.Contains("|(encode)")))
                        {
                            bool containsContextOrQueryMacro = validator.ContainsMacros(innerText, MacroValidator.MacroType.Custom);
                            if (containsContextOrQueryMacro)
                            {
                                string report = string.Format("Web part: {0}/{1}, property: {2} <br /> <strong>{3}</strong>.<br />",
                                        webPartNode.Attributes["controlid"].Value,
                                        webPartNode.Attributes["type"].Value,
                                        nameAttribute.Value,
                                        validator.HighlightMacros(HttpUtility.HtmlEncode(innerText), MacroValidator.MacroType.Custom));

                                res.Add(report);
                            }
                        }
                    }
                }
            }
            if (res.Any())
            {
                res.Insert(0, "<strong>Page template name: </strong>" + pageTemplateName);
            }
            return res;
        }


        #endregion
    }
}
