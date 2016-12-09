using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class WorkflowConsistencyModule : IModule
    {
        #region Properties

        private List<ClassItem> ClassItems { get; set; }
        private IInstanceInfo InstanceInfo { get; set; }

        #endregion

        #region Module config

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Workflow consistency",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0") },
                Comment = @"Checks if there are any inconsistencies between published data and data in CMS_Version history. Checks only custom fields stored in coupled table (excludes Document/Node properties)

Inconsistency can occur when a PUBLISHED document under WORKFLOW is updated in the API (when not creating/managing versions manually)

Implication of such inconsistency is that when you look at a document in Content tree, you see that it is published, but Live site shows different values. Solution is to re-save and re-publish the document
",
            };
        }

        #endregion

        #region Module results

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            InstanceInfo = instanceInfo;

            var result = GetInconsistenciesDataTable();

            return new ModuleResults
            {
                Result = result,
                Status = result.Rows.Count == 0 ? Status.Good : Status.Error
            };
        }

        #endregion

        #region Consistency methods

        private DataTable GetInconsistenciesDataTable()
        {
            var inconsistentDocuments = FindDocumentsWithInconsistencies();

            var result = new DataTable("Inconsistencies");
            result.Columns.Add("DocumentID", typeof(string));
            result.Columns.Add("NotMatchingFields", typeof(string));
            result.Columns.Add("DocumentCulture", typeof(string));
            result.Columns.Add("NodeAliasPath", typeof(string));

            foreach (var resultItem in inconsistentDocuments)
            {
                result.Rows.Add(resultItem.DocumentID, resultItem.NotMachingFieldsString, resultItem.DocumentCulture, resultItem.NodeAliasPath);
            }

            return result;
        }

        private List<ResultItem> FindDocumentsWithInconsistencies()
        {
            var inconsistentDocuments = new List<ResultItem>();

            // Initialize class names - needs to be called before any other helper method
            InitializeClassNames();

            // Load all documents
            var documents = GetDocuments();

            foreach (var document in documents)
            {
                var classItem = GetClassItem(document.ClassName);

                if (!classItem.ClassIsDocumentType || !classItem.ClassIsCoupledClass)
                {
                    // Skip processing of document that has no coupled table
                    continue;
                }

                // Get published data values
                var publishedValues = GetDictionaryWithValues(classItem, document.DocumentForeignKeyValue);

                // Get values from latest edited version
                var latestEditedValues = GetDictionaryWithValues(document.NodeXML);

                // Compare published values with latest edited values
                var matchResult = CompareDictionaries(publishedValues, latestEditedValues);

                if (matchResult.Count != 0)
                {
                    // Add inconsistent document to list
                    inconsistentDocuments.Add(new ResultItem(document.DocumentID, matchResult, document.DocumentCulture, document.NodeAliasPath));
                }
            }

            return inconsistentDocuments;
        }

        private List<string> CompareDictionaries(Dictionary<string, object> publishedValues, Dictionary<string, string> editedValues)
        {
            var notMatchingFields = new List<string>();

            // Check if values match JUST by checking values from PUBLISHED values which is containing just data from coupled table (no document specific data)
            foreach (var publishedItem in publishedValues)
            {
                // Check if the column is present in both published and edited dictionaries
                // Kentico does not store field in CMS_Version history if its value is set null
                if (!editedValues.ContainsKey(publishedItem.Key))
                {
                    // Check if published value is also empty
                    if (!string.IsNullOrEmpty(publishedItem.Value.ToString()))
                    {
                        // Published value is not empty, but Edited value is
                        notMatchingFields.Add(publishedItem.Key);
                    }
                    continue;
                }

                // Handle different types of values
                if (publishedItem.Value is DateTime)
                {
                    // Compare dates
                    DateTime publishedDate;
                    DateTime editedDate;

                    DateTime.TryParse(publishedItem.Value.ToString(), out publishedDate);
                    DateTime.TryParse(editedValues[publishedItem.Key], out editedDate);

                    bool datesMatch = publishedDate.CompareTo(editedDate) == 0;

                    if (!datesMatch)
                    {
                        notMatchingFields.Add(publishedItem.Key);
                    }

                }
                else if (publishedItem.Value is bool)
                {
                    bool publishedBool;
                    bool editedBool;

                    bool.TryParse(publishedItem.Value.ToString(), out publishedBool);
                    bool.TryParse(editedValues[publishedItem.Key], out editedBool);

                    bool boolsMatch = publishedBool.CompareTo(editedBool) == 0;

                    if (!boolsMatch)
                    {
                        notMatchingFields.Add(publishedItem.Key);
                    }
                }
                else
                {
                    // Check if the column has the same value as edited value
                    if (!editedValues.Contains(new KeyValuePair<string, string>(publishedItem.Key, publishedItem.Value.ToString())))
                    {
                        notMatchingFields.Add(publishedItem.Key);
                    }
                }
            }

            return notMatchingFields;
        }

        private Dictionary<string, string> GetDictionaryWithValues(string versionHistoryXML)
        {
            var dict = new Dictionary<string, string>();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(versionHistoryXML);

            XmlNodeList fields = xml.SelectNodes("/NewDataSet/Table1/*");

            foreach (XmlNode field in fields)
            {
                dict.Add(field.Name, field.InnerText);
            }

            return dict;
        }

        private Dictionary<string, object> GetDictionaryWithValues(ClassItem classItem, int documentForeignKeyValue)
        {
            var dict = new Dictionary<string, object>();

            // Get all columns from coupled table
            var sql = $"select * from {classItem.TableName} where {classItem.PrimaryKeyName} = '{documentForeignKeyValue}'";

            var result = InstanceInfo.DBService.ExecuteAndGetDataSet(sql);

            if (result.Tables[0].Rows.Count > 0)
            {
                DataRow row = result.Tables[0].Rows[0];

                // Add each field to dictionary
                foreach (var col in result.Tables[0].Columns)
                {
                    dict.Add(col.ToString(), row[col.ToString()]);
                }
            }

            return dict;
        }

        private List<DocumentItem> GetDocuments()
        {
            var list = new List<DocumentItem>();

            var result = InstanceInfo.DBService.ExecuteAndGetDataSetFromFile("WorkflowConsistencyGetDocuments.sql");

            foreach (DataRow documentItem in result.Tables[0].Rows)
            {
                list.Add(new DocumentItem
                {
                    DocumentID = Convert.IsDBNull(documentItem["DocumentID"]) ? 0 : Convert.ToInt32(documentItem["DocumentID"]),
                    DocumentName = documentItem["DocumentName"]?.ToString(),
                    ClassName = documentItem["ClassName"]?.ToString(),
                    DocumentForeignKeyValue = Convert.IsDBNull(documentItem["DocumentForeignKeyValue"]) ? 0 : Convert.ToInt32(documentItem["DocumentForeignKeyValue"]),
                    NodeXML = documentItem["NodeXML"].ToString(),
                    NodeAliasPath = documentItem["NodeAliasPath"]?.ToString(),
                    DocumentCulture = documentItem["DocumentCulture"]?.ToString()
                });
            }

            return list;
        }

        private void InitializeClassNames()
        {
            var sql = "select ClassName, ClassFormDefinition, ClassTableName, ClassIsCoupledClass, ClassIsDocumentType from CMS_Class";
            var result = InstanceInfo.DBService.ExecuteAndGetDataSet(sql);

            var list = new List<ClassItem>();

            foreach (DataRow classItem in result.Tables[0].Rows)
            {
                list.Add(new ClassItem
                {
                    ClassName = classItem["ClassName"].ToString(),
                    TableName = classItem["ClassTableName"].ToString(),
                    ClassFormDefinition = classItem["ClassFormDefinition"].ToString(),
                    ClassIsCoupledClass = Convert.ToBoolean(classItem["ClassIsCoupledClass"]),
                    ClassIsDocumentType = Convert.ToBoolean(classItem["ClassIsDocumentType"]),
                });
            }

            ClassItems = list;
        }

        private ClassItem GetClassItem(string className)
        {
            return ClassItems
                .FirstOrDefault(m => m.ClassName.Equals(className, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Private classes

        private class ResultItem
        {
            public int DocumentID { get; private set; }
            public List<string> NotMatchingFields { get; private set; }
            public string DocumentCulture { get; private set; }
            public string NodeAliasPath { get; private set; }

            public string NotMachingFieldsString => string.Join(";", NotMatchingFields);

            public ResultItem(int documentID, List<string> notMatchingFields, string documentCulture, string nodeAliasPath)
            {
                DocumentID = documentID;
                NotMatchingFields = notMatchingFields;
                DocumentCulture = documentCulture;
                NodeAliasPath = nodeAliasPath;
            }
        }

        private class DocumentItem
        {
            public int DocumentID { get; set; }
            public string DocumentName { get; set; }
            public string ClassName { get; set; }
            public int DocumentForeignKeyValue { get; set; }
            public string NodeXML { get; set; }
            public string DocumentCulture { get; set; }
            public string NodeAliasPath { get; set; }
        }

        private class ClassItem
        {
            public bool ClassIsDocumentType { get; set; }
            public bool ClassIsCoupledClass { get; set; }
            public string ClassName { get; set; }
            public string TableName { get; set; }
            public string ClassFormDefinition { get; set; }
            public string PrimaryKeyName => GetPrimaryKeyName();

            private string GetPrimaryKeyName()
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(ClassFormDefinition);

                XmlNode field = xml.SelectSingleNode("/form/field[@isPK='true']");

                return field.Attributes["column"].Value;
            }
        }

        #endregion
    }
}
