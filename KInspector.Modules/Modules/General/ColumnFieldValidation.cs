using Kentico.KInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml.Linq;

namespace Kentico.KInspector.Modules.Modules.General
{
    public class ColumnFieldValidation : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Column Field validation",
                SupportedVersions = new[] {
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0")
                },
                Comment = @"Compares Kentico class fields against table columns in database, and displays non-matching entries. Lists columns without class field and class fields without specified table column.",
                Category = "Database"
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;

            var classList = new List<ClassDefinition>();

            var classes = dbService.ExecuteAndGetTableFromFile("GetClassSchema.sql");
            foreach (DataRow classRow in classes.Rows)
            { 
                classList.Add(new ClassDefinition(classRow[0].ToString(), GetFieldsFromXmlSchema(classRow[1].ToString())));
            }

            var tableColumns = dbService.ExecuteAndGetTableFromFile("GetTableColumns.sql");

            var missingFromClassTable = new DataTable("Fields missing from Kentico Classes");
            missingFromClassTable.Columns.Add("Class Table Name");
            missingFromClassTable.Columns.Add("Field");

            var missingFromDatabaseTable = new DataTable("Columns missing from database tables");
            missingFromDatabaseTable.Columns.Add("Table Name");
            missingFromDatabaseTable.Columns.Add("Column");

            var issues = 0;
            foreach (var kenticoClass in classList)
            {
                var columns = GetColumnsFromDataRows(tableColumns.Rows, kenticoClass.ClassTableName);

                var missingFromClassList = columns.Except(kenticoClass.ClassFields).ToList();

                var missingFromDatabaseList = kenticoClass.ClassFields.Except(columns).ToList();

                foreach (var field in missingFromClassList)
                {
                    var row = missingFromClassTable.NewRow();
                    row["Class Table Name"] = kenticoClass.ClassTableName;
                    row["Field"] = field;
                    missingFromClassTable.Rows.Add(row);
                }


                foreach (var column in missingFromDatabaseList)
                {
                    var row = missingFromDatabaseTable.NewRow();
                    row["Table Name"] = kenticoClass.ClassTableName;
                    row["Column"] = column;
                    missingFromDatabaseTable.Rows.Add(row);
                }

                issues += missingFromDatabaseList.Count + missingFromClassList.Count;
            }

            var result = new DataSet("Non-matching Class Field/Table Column entries");

            if (missingFromClassTable.Rows.Count > 0)
            {
                result.Merge(missingFromClassTable);
            }
            if (missingFromDatabaseTable.Rows.Count > 0)
            {
                result.Merge(missingFromDatabaseTable);
            }


            return new ModuleResults
            {
                Result = result,
                ResultComment = $"{issues} invalid entries found",
                Status = (issues > 0) ? Status.Error : Status.Good
            };
        }

        // TODO: This is not the most robust or elegant solution to navigating through the xml schema.
        private List<string> GetFieldsFromXmlSchema(string xmlSchema)
        {
            try
            {
                var xmlDoc = XDocument.Parse(xmlSchema);

                var tableElement = xmlDoc.Root
                    .Elements()
                    .Descendants()
                    .Where(e => e.Attribute(XName.Get("name")) != null)
                    .FirstOrDefault();

                var fields = tableElement
                    .Elements()
                    .Descendants()
                    .Where(e => e.Attribute(XName.Get("name")) != null)
                    .Select(x => x.Attribute(XName.Get("name")).Value.ToLower());

                return fields.ToList();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        private List<string> GetColumnsFromDataRows(DataRowCollection rows, string tableName)
        {
            var columnList = new List<string>();
            foreach (DataRow dataRow in rows)
            {
                if(dataRow[0].ToString() == tableName)
                    columnList.Add(dataRow[1].ToString().ToLower());
            }

            return columnList;
        }
    }


    #region Helper classes
    public class Result
    {
        public string ClassTableName { get; set; }

        public List<string> MissingFromClass { get; set; }
        public List<string> MissingFromDatabase { get; set; }

    }

    public class ClassDefinition
    {
        public string ClassTableName { get; set; }

        public List<string> ClassFields { get; set; }

        public ClassDefinition(string classTableName, List<string> classFields)
        {
            ClassTableName = classTableName;
            ClassFields = classFields;
        }
    }
    #endregion
}

