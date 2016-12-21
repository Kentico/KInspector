using System;
using System.Collections;
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
        private static readonly Regex regexForCookie = new Regex("CookieHelper\\.GetValue");
        private static readonly Regex regexForCurrentUrl = new Regex("currenturl");
        private static readonly Regex regexForGetQuery = new Regex("URLHelper\\.GetQuery");
        private static readonly Regex regexForGetQueryValue = new Regex("URLHelper\\.GetQueryValue");
        private static readonly Regex regexForGetScript = new Regex("ScriptHelper\\.GetScript");
        private static readonly Regex regexForGetSring = new Regex("QueryHelper\\.GetString");
        private static readonly Regex regexForQueryString = new Regex("Request\\.QueryString");
        private static readonly Regex[] regexPatterns = { regexForQueryString, regexForGetSring, regexForCookie, regexForGetQueryValue, regexForGetQuery, regexForCurrentUrl, regexForGetScript };
        private IInstanceInfo instanceInfo;

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
            this.instanceInfo = instanceInfo;

            List<string> report = new List<string>();
            List<string> xssReport = new List<string>();
            List<string> customMacrosReport = new List<string>();

            var webPartConfigurations = GetWebPartConfigurationsForTemplates();

            var transformationInfos = GetTransformationInfo(webPartConfigurations);
            var checkForCustomMacros = MacroValidator.Current.CheckForCustomMacros(instanceInfo.Version);

            PerformAnalysis(transformationInfos, checkForCustomMacros, xssReport, customMacrosReport);

            report.Add("------------------------ Transformations - XSS Analysis report -----------------");
            report.AddRange(xssReport);
            report.Add("<br /><br />");

            if (customMacrosReport.Count > 0)
            {
                report.Add("------------------------ Transformations - Using deprecated Custom Macros -----------------");
                report.AddRange(customMacrosReport);
                report.Add("<br /><br />");
            }
            
            return new ModuleResults
            {
                Result = report,
                Trusted = true
            };
        }

        private void PerformAnalysis(List<TransformationInfo> transformationInfos, bool checkForCustomMacros, List<string> xssReport, List<string> customMacrosReport)
        {
            foreach (var transformationInfo in transformationInfos)
            {
                var xssResult = string.Empty;

                AnalyseXss(transformationInfo, xssReport);

                if (checkForCustomMacros)
                {
                    AnalyseCustomMacros(transformationInfo, customMacrosReport);
                }
            }
        }

        /// <summary>
        /// Analyse transformation for deprecated custom macros.
        /// </summary>
        /// <param name="transformationId">ID of the transformation.</param>
        /// <param name="transformationName">Name of the transformation.</param>
        /// <param name="transformationCode">Code of the transformation.</param>
        /// <param name="result">Result of deprecated custom macro analysis (not modified if none found).</param>
        private void AnalyseCustomMacros(TransformationInfo transformationInfo, List<string> report)
        {
            if (!string.IsNullOrWhiteSpace(transformationInfo.Code))
            {
                // Check if transformation code contains deprecated custom macros
                bool customMacrosFound = MacroValidator.Current.ContainsMacros(transformationInfo.Code, MacroValidator.MacroType.Custom);

                // If custom macros have been found, set appropriate result
                UpdateReport(transformationInfo, report, customMacrosFound);
            }
        }

        /// <summary>
        /// Analysis transformation code for XSS vulnerabilities.
        /// </summary>
        /// <param name="transformationId">ID of the transformation.</param>
        /// <param name="transformationName">Name of the transformation.</param>
        /// <param name="transformationCode">Code of the transformation.</param>
        /// <param name="result">Result of XSS vulnerability analysis (not modified if none found).</param>
        private void AnalyseXss(TransformationInfo transformationInfo, List<string> report)
        {
            var result = string.Empty;

            if (!string.IsNullOrWhiteSpace(transformationInfo.Code))
            {
                // Check if transformation code contains the malicious input
                bool potentialXssFound = regexPatterns.Any(p => p.IsMatch(transformationInfo.Code));

                // If potential XSS has been found, set appropriate result

                UpdateReport(transformationInfo, report, potentialXssFound);
            }
        }

        private void UpdateReport(TransformationInfo transformationInfo, List<string> report, bool issueFound)
        {
            if (issueFound)
            {
                report.Add(GetTransformationReportLink(transformationInfo));
            }
            else
            {
                report.Add($"Identified no issues in <em>{transformationInfo.FullName}</em> ({transformationInfo.ID})");
            }
        }

        private Dictionary<string, int> GetClassInfo(IEnumerable<string> classNames)
        {
            var classData = GetDataWhereInTable("KI_ClassNames", "ClassName", classNames);
            var classInfo = new Dictionary<string, int>();

            foreach (DataRow row in classData.Rows)
            {
                var classID = int.Parse(row["ClassID"].ToString());
                var className = row["ClassName"].ToString().ToLower();
                classInfo.Add(className, classID);
            }

            return classInfo;
        }

        private DataTable GetDataWhereInTable(DataTable table)
        {
            var tableValueParameter = new SqlParameter();
            tableValueParameter.ParameterName = "@TableValueParameter";
            tableValueParameter.SqlDbType = SqlDbType.Structured;
            tableValueParameter.TypeName = table.TableName;
            tableValueParameter.Value = table;

            instanceInfo.DBService.ExecuteAndGetTableFromFile($"TransformationAnalyzerModule-Initialize-{table.TableName}.sql");
            return instanceInfo.DBService.ExecuteAndGetTableFromFile($"TransformationAnalyzerModule-GetDataWhereIn-{table.TableName}.sql", tableValueParameter);
        }

        private DataTable GetDataWhereInTable(string dataTableName, string columnName, IEnumerable<object> items)
        {
            if (!items.Any())
            {
                return null;
            }

            var tableDefinition = new DataTable(dataTableName);
            tableDefinition.Columns.Add(columnName);

            foreach (var item in items)
            {
                tableDefinition.Rows.Add(item);
            }

            return GetDataWhereInTable(tableDefinition);
        }

        /// <summary>
        /// Gets full names of transformations used in page template web parts.
        /// </summary>
        /// <param name="pageTemplateWebParts">Content of PageTemplateWebParts column from <c>CMS_PageTemplate</c> table.</param>
        /// <returns>Enumeration of transformation full names.</returns>
        private HashSet<string> GetFullTransformationNamesFromWebPartConfiguration(XmlDocument webPartConfiguration)
        {
            HashSet<string> results = new HashSet<string>();

            foreach (XmlNode webPartPropertyNode in webPartConfiguration.SelectNodes("/page/webpartzone/webpart/property"))
            {
                XmlAttribute nameAttribute = webPartPropertyNode.Attributes["name"];

                var isValidAttribute = nameAttribute != null && nameAttribute.Value.Contains("transformation");
                var hasValue = !string.IsNullOrEmpty(webPartPropertyNode.InnerText);

                if (isValidAttribute && hasValue)
                {
                    results.Add(webPartPropertyNode.InnerText.ToLower());
                }
            }

            return results;
        }

        private HashSet<string> GetFullTransformationNamesFromWebPartConfigurations(IEnumerable<XmlDocument> webPartConfigurations)
        {
            var fullTransformationNames = new HashSet<string>();
            foreach (var configurationXml in webPartConfigurations)
            {
                var fullTransformationNamesInWebPartConfiguration = GetFullTransformationNamesFromWebPartConfiguration(configurationXml);
                foreach (var fullTransformationName in fullTransformationNamesInWebPartConfiguration)
                {
                    fullTransformationNames.Add(fullTransformationName);
                }
            }

            return fullTransformationNames;
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

        private List<TransformationInfo> GetTransformationInfo(IEnumerable<XmlDocument> webPartConfigurations)
        {
            HashSet<string> fullTransformationNames = GetFullTransformationNamesFromWebPartConfigurations(webPartConfigurations);
            return GetTransformationInfo(fullTransformationNames);
        }

        private List<TransformationInfo> GetTransformationInfo(IEnumerable<string> fullTransformationNames)
        {
            var transformationInfos = new List<TransformationInfo>();
            foreach (var fullTransformationName in fullTransformationNames)
            {
                var name = fullTransformationName.Substring(fullTransformationName.LastIndexOf('.') + 1);
                var className = fullTransformationName.Substring(0, fullTransformationName.LastIndexOf('.'));

                var transformationInfo = new TransformationInfo()
                {
                    FullName = fullTransformationName,
                    Name = name,
                    ClassName = className,
                };

                transformationInfos.Add(transformationInfo);
            }

            var classNames = transformationInfos.Select(x => x.ClassName).Distinct();
            var classDetails = GetClassInfo(classNames);

            foreach (var transformationInfo in transformationInfos)
            {
                int classID = 0;
                classDetails.TryGetValue(transformationInfo.ClassName, out classID);
                transformationInfo.ClassID = classID;
            }

            var transformationTable = GetTransformationListTable(transformationInfos);
            var transformationInfoData = GetDataWhereInTable(transformationTable);

            foreach (DataRow item in transformationInfoData.Rows)
            {
                var id = int.Parse(item["TransformationID"].ToString());
                var classID = int.Parse(item["TransformationClassID"].ToString());
                var name = item["TransformationName"].ToString();
                var code = item["TransformationCode"].ToString();

                var index = transformationInfos.FindIndex(ti => ti.Name.ToLower() == name.ToLower() && ti.ClassID == classID);
                transformationInfos[index].ID = id;
                transformationInfos[index].Code = code;
            }

            return transformationInfos;
        }

        private DataTable GetTransformationListTable(List<TransformationInfo> transformationInfos)
        {
            if (!transformationInfos.Any())
            {
                return null;
            }

            var table = new DataTable("KI_TransformationList");
            table.Columns.Add("TransformationName");
            table.Columns.Add("TransformationClassID");

            foreach (var transformationInfo in transformationInfos)
            {
                table.Rows.Add(transformationInfo.Name, transformationInfo.ClassID);
            }

            return table;
        }

        /// <summary>
        /// Gets the transformation report links.
        /// </summary>
        /// <param name="transformationId">ID of the transformation.</param>
        /// <param name="transformationName">Name of the transformation.</param>
        /// <param name="transformationCode">Code of the transformation.</param>
        /// <returns>Report with possible links for given transformation.</returns>
        private string GetTransformationReportLink(TransformationInfo transformationInfo)
        {
            StringBuilder res = new StringBuilder();
            res.Append("<a href=\"").Append(instanceInfo.Uri)
                .Append("/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_Edit.aspx?objectid=")
                .Append(transformationInfo.ID)
                .Append("\" target=\"_blank\">")
                .Append(transformationInfo.FullName)
                .Append("</a> ");

            return res.ToString();
        }

        /// <summary>
        /// Gets web part configurations for page templates where display name
        /// is like <paramref name="pageTemplateDisplayNameLike"/>.
        /// </summary>
        /// <param name="pageTemplateDisplayNameLike">SQL Like pattern for page template display name. Default is all</param>
        /// <returns>DataTable containing page template web parts in its 'PageTemplateWebParts' column.</returns>
        private List<XmlDocument> GetWebPartConfigurationsForTemplates(string pageTemplateDisplayNameLike = "%")
        {
            var results = new List<XmlDocument>();

            var sqlFile = "TransformationAnalyzerModule-PageTemplateWebParts.sql";
            var sqlParameter = new SqlParameter("PageTemplateDisplayName", pageTemplateDisplayNameLike);
            var webPartConfigurationsData = instanceInfo.DBService.ExecuteAndGetTableFromFile(sqlFile, sqlParameter);
            foreach (DataRow webPartConfiguration in webPartConfigurationsData.Rows)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webPartConfiguration["PageTemplateWebParts"].ToString());
                results.Add(xmlDoc);
            }

            return results;
        }
    }

    public class TransformationInfo
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
    }
}