using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ColumnFieldValidation.Models;
using KenticoInspector.Reports.ColumnFieldValidation.Models.Data;
using KenticoInspector.Reports.ColumnFieldValidation.Models.Results;

namespace KenticoInspector.Reports.ColumnFieldValidation
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(
            IDatabaseService databaseService,
            IModuleMetadataService moduleMetadataService
            ) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Consistency,
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var cmsClasses = databaseService.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClasses);

            var classTableNames = cmsClasses
                .Select(cmsClass => cmsClass.ClassTableName);

            var tableColumns = databaseService.ExecuteSqlFromFile<TableColumn>(
                Scripts.GetTableColumns,
                new { classTableNames }
                );

            var cmsClassesWithAddedFields = GetCmsClassesWithAddedFields(cmsClasses, tableColumns);
            var tablesWithAddedColumns = GetTablesWithAddedColumns(tableColumns, cmsClasses);

            return CompileResults(cmsClassesWithAddedFields, tablesWithAddedColumns);
        }

        private IEnumerable<CmsClassResult> GetCmsClassesWithAddedFields(
            IEnumerable<CmsClass> cmsClasses,
            IEnumerable<TableColumn> tableColumns
            )
        {
            foreach (var cmsClass in cmsClasses)
            {
                var classFieldNameTypes = cmsClass.ClassXmlSchema
                    .Descendants()
                    .Where(element => element.Name.LocalName == "element")
                    .Where(element => element.Attribute("name").Value != "NewDataSet")
                    .Where(element => element.Attribute("name").Value != cmsClass.ClassTableName)
                    .Select(GetClassFieldNameType);

                var tableColumnNameTypes = tableColumns
                    .Where(tableColumn => tableColumn.Table_Name.Equals(
                        cmsClass.ClassTableName,
                        StringComparison.InvariantCultureIgnoreCase
                        ));

                var addedFields = classFieldNameTypes
                    .Where(classFieldNameType => !tableColumnNameTypes
                        .Any(tableColumnNameType => classFieldNameType.Name == tableColumnNameType.Column_Name
                            && classFieldNameType.Type.StartsWith(tableColumnNameType.Data_Type)));

                if (addedFields.Any())
                {
                    yield return new CmsClassResult()
                    {
                        ClassDisplayName = cmsClass.ClassDisplayName,
                        ClassFieldsNotInTable = string.Join(", ", addedFields),
                        ClassID = cmsClass.ClassID,
                        ClassName = cmsClass.ClassName,
                        ClassTableName = cmsClass.ClassTableName
                    };
                }
            }
        }

        private IEnumerable<TableResult> GetTablesWithAddedColumns(
            IEnumerable<TableColumn> tableColumns,
            IEnumerable<CmsClass> cmsClasses
            )
        {
            var tableColumnGroups = tableColumns
                .GroupBy(tableColumn => tableColumn.Table_Name);

            foreach (var tableColumnNameTypes in tableColumnGroups)
            {
                var matchingCmsClass = cmsClasses
                    .First(cmsClass => cmsClass.ClassTableName.Equals(
                        tableColumnNameTypes.Key,
                        StringComparison.InvariantCultureIgnoreCase
                        ));

                var classFieldNameTypes = matchingCmsClass.ClassXmlSchema
                    .Descendants()
                    .Where(element => element.Name.LocalName == "element")
                    .Where(element => element.Attribute("name").Value != "NewDataSet")
                    .Where(element => element.Attribute("name").Value != matchingCmsClass.ClassTableName)
                    .Select(GetClassFieldNameType);

                var addedColumns = tableColumnNameTypes
                    .Where(tableColumnNameType => !classFieldNameTypes
                        .Any(classFieldNameType => classFieldNameType.Name == tableColumnNameType.Column_Name
                            && classFieldNameType.Type.StartsWith(tableColumnNameType.Data_Type)))
                    .Select(column => (column.Column_Name, column.Data_Type));

                if (addedColumns.Any())
                {
                    yield return new TableResult()
                    {
                        TableColumnsNotInClass = string.Join(", ", addedColumns),
                        TableName = tableColumnNameTypes.Key
                    };
                }
            }
        }

        private (string Name, string Type) GetClassFieldNameType(XElement element)
        {
            string name = element.Attribute("name").Value;
            string type = string.Empty;

            if (element.Attribute("type") != null)
            {
                type = element.Attribute("type").Value;
            }

            var attributeDataType = element
                .Attributes()
                .FirstOrDefault(attribute => attribute.Name.LocalName == "DataType");

            if (attributeDataType != null)
            {
                type = attributeDataType.Value;
            }

            var childElementWithType = element
                .Descendants()
                .FirstOrDefault(childElement => childElement.Name.LocalName == "restriction");

            if (childElementWithType?.Attribute("base") != null)
            {
                type = childElementWithType.Attribute("base").Value;
            }

            switch (type)
            {
                case "xs:int":
                    type = "int";
                    break;

                case "xs:long":
                    type = "bigint";
                    break;

                case "xs:double":
                    type = "float";
                    break;

                case "xs:decimal":
                    type = "decimal";
                    break;

                case "xs:string":
                    type = "nvarchar";
                    break;

                case "xs:dateTime":
                    type = "datetime2";
                    break;

                case "xs:boolean":
                    type = "bit";
                    break;

                case "xs:base64Binary":
                    type = "varbinary";
                    break;

                case "System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089":
                    type = "uniqueidentifier";
                    break;
            }

            return (name, type);
        }

        private ReportResults CompileResults(
            IEnumerable<CmsClassResult> cmsClassesWithAddedFields,
            IEnumerable<TableResult> tablesWithAddedColumns
            )
        {
            if (!cmsClassesWithAddedFields.Any() && !tablesWithAddedColumns.Any())
            {
                return new ReportResults()
                {
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var errorReportResults = new ReportResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Error
            };

            var cmsClassesResultCount = IfAnyAddTableResult(
                errorReportResults.Data,
                cmsClassesWithAddedFields,
                Metadata.Terms.TableTitles.ClassesWithAddedFields
            );

            var tablesResultCount = IfAnyAddTableResult(
                errorReportResults.Data,
                tablesWithAddedColumns,
                Metadata.Terms.TableTitles.TablesWithAddedColumns
            );

            errorReportResults.Summary = Metadata.Terms.Summaries.Error.With(new
            {
                cmsClassesResultCount,
                tablesResultCount
            });

            return errorReportResults;
        }

        private static int IfAnyAddTableResult<T>(dynamic data, IEnumerable<T> results, Term tableNameTerm)
        {
            if (results.Any())
            {
                var tableResult = new TableResult<T>
                {
                    Name = tableNameTerm,
                    Rows = results
                };

                IDictionary<string, object> dictionaryData = data;

                dictionaryData.Add(tableNameTerm, tableResult);
            }

            return results.Count();
        }
    }
}