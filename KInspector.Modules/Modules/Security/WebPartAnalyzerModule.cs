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

        private DatabaseService mDatabaseService;
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
                Comment = "Analyses possible vulnerabilities in web parts.",
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

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            List<string> report = new List<string>();

            mDatabaseService = instanceInfo.DBService;

            DataTable webPartsInTransformationsTable = GetPageTemplateWebParts(LikePageTemplateDisplayName);
            List<string> whereOrderResults = new List<string>();
            List<string> otherResults = new List<string>();
            foreach (DataRow webPart in webPartsInTransformationsTable.Rows)
            {
                string pageTemplateDisplayName = webPart["PageTemplateDisplayName"] as string;
                XmlDocument webPartsXmlDoc = new XmlDocument();
                webPartsXmlDoc.LoadXml(webPart["PageTemplateWebParts"] as string);

                whereOrderResults.AddRange(AnalyseWhereAndOrderByConditionsInPageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName));
                otherResults.AddRange(AnalysePageTemplateWebParts(webPartsXmlDoc, pageTemplateDisplayName));
            }

            if (whereOrderResults.Count > 0)
            {
                report.Add("------------------------ Web parts - Where and Order condition results - Potential SQL injections -----------------");
                report.AddRange(whereOrderResults);
            }
            if (otherResults.Count > 0)
            {
                report.Add("------------------------ Macros in DB - Potential XSS -----------------");
                report.AddRange(otherResults);
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

            foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
            {
                foreach(XmlNode propertyNode in webPartNode.SelectNodes("property"))
                {
                    XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                    string innerText = propertyNode.InnerText;
                    if ((nameAttribute != null) && (nameAttribute.Value.Contains("where") || nameAttribute.Value.Contains("order")) && (!String.IsNullOrEmpty(innerText) && (!innerText.Contains("ToInt"))))
                    {
                        bool containsContextOrQueryMacro = innerText.Contains("{?") || innerText.Contains("{%");
                        if (containsContextOrQueryMacro)
                        {
                            string report = String.Format("Web part: {0}/{1}, property: {2} <br /> <strong>{3}</strong>.<br />",
                                    webPartNode.Attributes["controlid"].Value,
                                    webPartNode.Attributes["type"].Value,
                                    nameAttribute.Value,
                                    HighlightMacros(HttpUtility.HtmlEncode(innerText)));

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


        /// <summary>
        /// Analyses page template web parts. Searches for all properties of web parts except
        /// <c>where</c> and <c>order</c>, which have their value defined using a macro.
        /// </summary>
        /// <param name="pageTemplateWebParts">Page template web parts XML.</param>
        /// <returns>Enumeration of analysis results.</returns>
        private IEnumerable<string> AnalysePageTemplateWebParts(XmlDocument pageTemplateWebParts, string pageTemplateName)
        {
            List<string> res = new List<string>();

            foreach (XmlNode webPartNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart"))
            {
                foreach (XmlNode propertyNode in webPartNode.SelectNodes("property"))
                {
                    XmlAttribute nameAttribute = propertyNode.Attributes["name"];
                    string innerText = propertyNode.InnerText;
                    if ((nameAttribute != null) && (nameAttribute.Value.Contains("text") || nameAttribute.Value.Contains("content")) && (!String.IsNullOrEmpty(innerText) && !innerText.Contains("|(encode)")))
                    {
                        bool containsContextOrQueryMacro = innerText.Contains("{?") || innerText.Contains("{%");
                        if (containsContextOrQueryMacro)
                        {
                            string report = String.Format("Web part: {0}/{1}, property: {2} <br /> <strong>{3}</strong>.<br />",
                                    webPartNode.Attributes["controlid"].Value,
                                    webPartNode.Attributes["type"].Value,
                                    nameAttribute.Value,
                                    HighlightMacros(HttpUtility.HtmlEncode(innerText)));

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


        /// <summary>
        /// Highlights context and query macros in <paramref name="code"/>
        /// using HTML syntax.
        /// </summary>
        /// <param name="code">Code with macros.</param>
        /// <returns>Code with HTML highlighted macros.</returns>
        private string HighlightMacros(string code)
        {
            var contextMacros = GetMacros(code, "%");
            var queryMacros = GetMacros(code, "?");
            var macros = contextMacros.Concat(queryMacros);

            foreach (string macro in macros)
            {
                code = code.Replace(macro, "<span style=\"color: red;\">" + macro + "</span>");
            }

            return code;
        }


        /// <summary>
        /// Gets macro expressions from <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Code to be searched for macro expressions.</param>
        /// <param name="macroType">Macro type to be searched (e.g. "%", "?").</param>
        /// <param name="startIndex">Position from which to start the search.</param>
        /// <param name="matches">Set of matches to which the macros are added.</param>
        /// <returns></returns>
        private ISet<string> GetMacros(string code, string macroType, int startIndex = 0, ISet<string> matches = null)
        {
            if (matches == null)
            {
                matches = new HashSet<string>(); 
            }

            int start = code.IndexOf("{" + macroType, startIndex);
            if (start >= 0)
            {
                int end = code.IndexOf(macroType + "}", start + 2);

                if (end >= 0)
                {
                    string macroExpression = code.Substring(start, end - start + 2);
                    matches.Add(macroExpression);

                    return GetMacros(code, macroType, end + 2, matches);
                }
            }

            return matches;
        }

        #endregion
    }
}
