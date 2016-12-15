using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class SiteTemplatesModule : IModule
    {
        #region "Inner classes"

        [Serializable]
        [XmlRoot("page")]
        public class TemplateWebParts
        {
            [XmlElement("webpartzone")]
            public WebPartZone[] WebPartZones { get; set; }
        }

        [Serializable]
        public class WebPartZone
        {
            [XmlAttribute("id")]
            public string ID { get; set; }

            [XmlElement("webpart")]
            public WebPart[] WebParts { get; set; }
        }

        [Serializable]
        public class WebPart
        {
            [XmlAttribute("controlid")]
            public string ControlID { get; set; }

            [XmlAttribute("type")]
            public string Type { get; set; }

            [XmlElement("property")]
            public Property[] Properties { get; set; }
        }

        [Serializable]
        public class Property
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlText]
            public string Value { get; set; }
        }

        #endregion


        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Page template overview",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Shows the basic web part configuration for all page templates and lists all the documents that use that template.",
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var webPartsWithColumns = dbService.ExecuteAndGetTableFromFile("SiteTemplatesModule-WebPartsWithColumns.sql");

            var templates = dbService.ExecuteAndGetTableFromFile("SiteTemplatesModule-Templates.sql");

            DataSet results = new DataSet();
            bool duplicateTemplateCodeName = false;
            foreach (DataRow template in templates.Rows)
            {
                TemplateWebParts templateWP = GetTemplateWebPartsFromXML(template["PageTemplateWebParts"].ToString());
                string templateName = template["PageTemplateCodeName"].ToString();
                if (results.Tables.Contains(templateName))
                {
                    // Page template code names should be unique
                    templateName += $" - DUPLICATE CODENAME, ID: {template["PageTemplateID"]}";
                    duplicateTemplateCodeName = true;
                }

                var tableHeader = $"{template["PageTemplateDisplayName"]} [{templateName}]";

                DataTable result = GetTableForTemplateResult(tableHeader);

                if (templateWP.WebPartZones != null)
                {
                    foreach (var zone in templateWP.WebPartZones)
                    {
                        if (zone.WebParts == null || zone.WebParts.Length == 0)
                        {
                            var row = result.NewRow();
                            row["WebPartTitle"] = "No web parts";
                            row["Zone"] = zone.ID;
                            result.Rows.Add(row);
                            continue;
                        }
                        foreach (var wp in zone.WebParts)
                        {
                            var row = result.NewRow();
                            row["WebPartTitle"] = GetWebPartPropertyValue(wp, "webparttitle");
                            row["WebPartType"] = wp.Type;
                            row["ID"] = wp.ControlID;
                            row["Zone"] = zone.ID;
                            row["ContentCache"] = GetWebPartPropertyValue(wp, "cacheminutes");
                            row["PartialCache"] = GetWebPartPropertyValue(wp, "partialcacheminutes");
                            row["ViewStateDisabled"] = GetWebPartPropertyValue(wp, "disableviewstate");

                            DataRow dr = webPartsWithColumns.Select("WebPartName = '" + wp.Type + "'").FirstOrDefault();
                            if (dr != null)
                            {
                                if ((int)dr["Columns"] == 1)
                                {
                                    string colsVal = GetWebPartPropertyValue(wp, "columns");
                                    row["Columns"] = string.IsNullOrEmpty(colsVal) ? "NOT SET" : colsVal;
                                }

                                if ((int)dr["TopN"] == 1)
                                {
                                    string topVal = GetWebPartPropertyValue(wp, "SelectTopN");
                                    if (string.IsNullOrEmpty(topVal))
                                    {
                                        topVal = GetWebPartPropertyValue(wp, "TopN");
                                    }

                                    row["TopN"] = string.IsNullOrEmpty(topVal) ? "NOT SET" : topVal;
                                }
                            }

                            result.Rows.Add(row);
                        }
                    }
                }
                else
                {
                    var row = result.NewRow();
                    row["WebPartTitle"] = "No web part zones";
                    result.Rows.Add(row);
                }
                
                results.Tables.Add(result);

                var documents = dbService.ExecuteAndGetTableFromFile("SiteTemplatesModule-Documents.sql", 
                    new SqlParameter("PageTemplateID", template["PageTemplateID"]));
                documents.TableName = $"{tableHeader} - Documents";
                results.Tables.Add(documents.Copy());
            }

            var moduleResults = new ModuleResults
            {
                Result = results,
            };

            if (duplicateTemplateCodeName)
            {
                moduleResults.Status = Status.Warning;
                moduleResults.ResultComment = "Duplicate template code name(s) found, incorrect item(s) are denoted by 'DUPLICATE CODENAME' in its name.";
            }

            return moduleResults;
        }


        protected TemplateWebParts GetTemplateWebPartsFromXML(string xml)
        {
            XmlSerializer sr = new XmlSerializer(typeof(TemplateWebParts));
            using (var reader = new StringReader(xml))
            {
                return (TemplateWebParts)sr.Deserialize(reader);
            }
        }

        protected DataTable GetTableForTemplateResult(string tableName)
        {
            DataTable result = new DataTable(tableName);
            result.Columns.Add("WebPartTitle");
            result.Columns.Add("WebPartType");
            result.Columns.Add("ID");
            result.Columns.Add("Zone");
            result.Columns.Add("ContentCache");
            result.Columns.Add("PartialCache");
            result.Columns.Add("ViewStateDisabled");
            result.Columns.Add("TOPN");
            result.Columns.Add("Columns");
            
            return result;
        }


        protected string GetWebPartPropertyValue(WebPart wp, string propertyName)
        {
            var prop = wp.Properties.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            return prop == null ? null : prop.Value;
        }
    }
}
