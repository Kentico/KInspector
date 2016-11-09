using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class TransformationAnalyzerModule : IModule
    {
        #region "Constants"

        private static readonly Regex queryRegex = new Regex("QueryHelper\\.GetString");
        private static readonly Regex requestRegex = new Regex("Request\\.QueryString");
        private static readonly Regex cookieRegex = new Regex("CookieHelper\\.GetValue");
        private static readonly Regex getQueryRegex = new Regex("URLHelper\\.GetQueryValue");
        private static readonly Regex getQuery = new Regex("URLHelper\\.GetQuery");
        private static readonly Regex currentUrlRegex = new Regex("currenturl");
        private static readonly Regex getStringRegex = new Regex("ScriptHelper\\.GetScript");


        /// <summary>
        /// Array of regular expressions used for transformation analysis.
        /// </summary>
        private static readonly Regex[] patterns = { queryRegex, requestRegex, cookieRegex, getQueryRegex, getQuery, currentUrlRegex, getStringRegex };

        #endregion


        #region "Fields"

        private IDatabaseService mDatabaseService;
        private string mInstancePath;
        HashSet<string> mTransformationFullNames;
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
        public string LikePageTemplateDisplayName { get; set; } = "%";
        #endregion


        #region "IModule interface methods"

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Transformation analyzer",
                Comment = "Analyzes possible XSS vulnerabilities in transformations.",
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

            List<string> xssReport = new List<string>();
            List<string> customMacrosReport = new List<string>();

            mDatabaseService = instanceInfo.DBService;
            mInstancePath = instanceInfo.Directory.FullName;

            HashSet<string> transformationNames = new HashSet<string>();
            mTransformationFullNames = new HashSet<string>();

            DataTable webPartsInTransformationsTable = GetPageTemplateWebParts(LikePageTemplateDisplayName);
            foreach (DataRow webPart in webPartsInTransformationsTable.Rows)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webPart["PageTemplateWebParts"] as string);

                IEnumerable<string> templateTransformationFullNames = GetTransformationNamesInPageTemplateWebParts(xmlDoc);

                foreach (string templateTransformationFullName in templateTransformationFullNames)
                {
                    mTransformationFullNames.Add(templateTransformationFullName);
                    string transformationName = templateTransformationFullName.Substring(templateTransformationFullName.LastIndexOf('.') + 1);
                    transformationNames.Add(transformationName);
                }
            }

            DataTable transformationCodesTable = GetTransformationCodes(transformationNames);
            bool checkForCustomMacros = MacroValidator.Current.CheckForCustomMacros(instanceInfo.Version);
            foreach (DataRow transformation in transformationCodesTable.Rows)
            {
                int transformationId = (int)transformation["TransformationID"];
                string transformationName = transformation["TransformationName"] as string;
                string transformationCode = transformation["TransformationCode"] as string;

                string xssResult = null;
                AnalyseXss(transformationId, transformationName, transformationCode, ref xssResult);
                if (!string.IsNullOrEmpty(xssResult))
                {
                    xssReport.Add(xssResult);
                }

                if (checkForCustomMacros)
                {
                    string customMacroResult = null;
                    AnalyseCustomMacros(transformationId, transformationName, transformationCode, ref customMacroResult);
                    if (!string.IsNullOrEmpty(customMacroResult))
                    {
                        customMacrosReport.Add(customMacroResult);
                    }
                }
            }

            if (xssReport.Count > 0)
            {
                report.Add("------------------------ Transformations - XSS Analysis report -----------------");
                report.AddRange(xssReport);
                report.Add("<br /><br />");
            }

            if (customMacrosReport.Count > 0)
            {
                report.Add("------------------------ Transformations - Using deprecated Custom Macros -----------------");
                report.AddRange(customMacrosReport);
                report.Add("<br /><br />");
            }

            if (report.Count == 0)
            {
                return new ModuleResults
                {
                    ResultComment = "No problems in transformations found.",
                    Status = Status.Good
                };
            }

            return new ModuleResults
            {
                Result = report,
                Trusted = true
            };
        }

        #endregion


        #region "Methods"

        /// <summary>
        /// Analysis transformation code for XSS vulnerabilities.
        /// </summary>
        /// <param name="transformationId">ID of the transformation.</param>
        /// <param name="transformationName">Name of the transformation.</param>
        /// <param name="transformationCode">Code of the transformation.</param>
        /// <param name="result">Result of XSS vulnerability analysis (not modified if none found).</param>
        private void AnalyseXss(int transformationId, string transformationName, string transformationCode, ref string result)
        {
            // Check if transformation code contains the malicious input
            bool potentialXssFound = patterns.Any(p => p.IsMatch(transformationCode));

            // If potential XSS has been found, set appropriate result
            if (potentialXssFound)
            {
                result = GetTransformationReportLink(transformationId, transformationName, transformationCode);
            }
        }


        /// <summary>
        /// Analyse transformation for deprecated custom macros.
        /// </summary>
        /// <param name="transformationId">ID of the transformation.</param>
        /// <param name="transformationName">Name of the transformation.</param>
        /// <param name="transformationCode">Code of the transformation.</param>
        /// <param name="result">Result of deprecated custom macro analysis (not modified if none found).</param>
        private void AnalyseCustomMacros(int transformationId, string transformationName, string transformationCode, ref string result)
        {
            // Check if transformation code contains deprecated custom macros
            bool customMacrosFound = MacroValidator.Current.ContainsMacros(transformationCode, MacroValidator.MacroType.Custom);

            // If custom macros have been found, set appropriate result
            if (customMacrosFound)
            {
                result = GetTransformationReportLink(transformationId, transformationName, transformationCode);
            }
        }


        /// <summary>
        /// Gets page template web parts where page template display name
        /// is like given <paramref name="likePageTemplateDisplayName"/> pattern.
        /// </summary>
        /// <param name="likePageTemplateDisplayName">Like pattern for page template display name.</param>
        /// <returns>DataTable containing page template web parts in its 'PageTemplateWebParts' column.</returns>
        private DataTable GetPageTemplateWebParts(string likePageTemplateDisplayName)
        {
            return mDatabaseService.ExecuteAndGetTableFromFile("TransformationAnalyzerModule-PageTemplateWebParts.sql",
                                new SqlParameter("PageTemplateDisplayName", likePageTemplateDisplayName));
        }


        /// <summary>
        /// Gets full names of transformations used in page template web parts.
        /// </summary>
        /// <param name="pageTemplateWebParts">Content of PageTemplateWebParts column from <c>CMS_PageTemplate</c> table.</param>
        /// <returns>Enumeration of transformation full names.</returns>
        private IEnumerable<string> GetTransformationNamesInPageTemplateWebParts(XmlDocument pageTemplateWebParts)
        {
            HashSet<string> res = new HashSet<string>();

            foreach (XmlNode webPartPropertyNode in pageTemplateWebParts.SelectNodes("/page/webpartzone/webpart/property"))
            {
                XmlAttribute nameAttribute = webPartPropertyNode.Attributes["name"];
                if ((nameAttribute != null) && nameAttribute.Value.Contains("transformation") && !string.IsNullOrEmpty(webPartPropertyNode.InnerText))
                {
                    res.Add(webPartPropertyNode.InnerText);
                }
            }

            return res;
        }


        /// <summary>
        /// Gets table with transformation codes.
        /// </summary>
        /// <param name="transformationNames">Enumeration of transformation names.</param>
        /// <returns>Table of transformation codes, or null if <paramref name="transformationNames"/> does not contain any item).</returns>
        private DataTable GetTransformationCodes(IEnumerable<string> transformationNames)
        {
            if (!transformationNames.Any())
            {
                return null;
            }

            string listOfNames = "'" + string.Join("', '", transformationNames) + "'";

            return mDatabaseService.ExecuteAndGetTableFromFile("TransformationAnalyzerModule-TransformationCodes.sql",
                            new SqlParameter("ListOfNames", listOfNames));
        }


        /// <summary>
        /// Gets enumeration of possible full names for given transformation name
        /// (the mapping can be ambiguous in some cases, but the original power shell script
        /// does not care since multiple matches are rather rare).
        /// </summary>
        /// <param name="transformationName">Transformation name.</param>
        /// <param name="transformationFullNames">Enumeration of transformation full names.</param>
        /// <returns>Enumeration of full names which match the transformation name.</returns>
        private IEnumerable<string> GetTransformationFullNamesForName(string transformationName, IEnumerable<string> transformationFullNames)
        {
            return transformationFullNames.Where(it => it.EndsWith("." + transformationName));
        }


        /// <summary>
        /// Gets the transformation report links.
        /// </summary>
        /// <param name="transformationId">ID of the transformation.</param>
        /// <param name="transformationName">Name of the transformation.</param>
        /// <param name="transformationCode">Code of the transformation.</param>
        /// <returns>Report with possible links for given transformation.</returns>
        private string GetTransformationReportLink(int transformationId, string transformationName, string transformationCode)
        {
            IEnumerable<string> fullNames = GetTransformationFullNamesForName(transformationName, mTransformationFullNames);
            StringBuilder res = new StringBuilder();
            res.Append(transformationName).Append(" ");
            foreach (string fullName in fullNames)
            {
                res.Append("<a href=\"").Append(mInstancePath)
                    .Append("/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_Edit.aspx?objectid=")
                    .Append(transformationId)
                    .Append("\" target=\"_blank\">")
                    .Append(fullName)
                    .Append("</a> ");
            }
            return res.ToString();
        }

        #endregion
    }
}