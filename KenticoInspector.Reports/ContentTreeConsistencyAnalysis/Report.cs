using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class Report : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public string Codename => "Content-Tree-Consistency-Analysis";

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"
        <p>This report checks for the following consistency issues:</p>
        <ul>
            <li>Tree nodes with a bad parent site</li>
            <li>Tree nodes with a bad parent node</li>
            <li>Tree nodes with level inconsistency based on the number of <code>/</code> in the alias path compared to it’s level</li>
            <li>Tree nodes with level inconsistency based on the whether it’s parent node has a level one higher than it does</li>
            <li>Tree node duplicates based on alias path</li>
            <li>Tree nodes with no document node</li>
            <li>Tree nodes with page type not assigned to site</li>
            <li>Document nodes with no tree node</li>
            <li>Validates that the published data and the data in the CMS_VersionHistory tables match</li>
        </ul>
        ";

        public string Name => "Content Tree Consistency Analysis";

        public string ShortDescription => "Performs consistency analysis for items in the content tree";

        public IList<string> Tags => new List<string>()
        {
            ReportTags.Health,
            ReportTags.Consistency
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            _databaseService.ConfigureForInstance(instance);

            var treeNodeWithBadParentSiteResults = GetTreeNodeTestResult("Tree nodes with a bad parent site", Scripts.GetTreeNodeIdsWithBadParentSiteId);
            var treeNodeWithBadParentNodeResults = GetTreeNodeTestResult("Tree nodes with a bad parent node", Scripts.GetTreeNodeIdsWithBadParentNodeId);
            var treeNodeWithLevelInconsistencyAliasatPathTestResults = GetTreeNodeTestResult("Tree nodes with level inconsistency (alias path test)", Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest);
            var treeNodeWithLevelInconsistencyParentChildLevelTestResults = GetTreeNodeTestResult("Tree Nodes with level inconsistency (parent/child level test)", Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest);
            var treeNodeWithMissingDocumentResults = GetTreeNodeTestResult("Tree nodes with no document node", Scripts.GetTreeNodeIdsWithMissingDocument);
            var treeNodeWithDuplicateAliasPathResults = GetTreeNodeTestResult("Tree nodes with duplicated alias paths", Scripts.GetTreeNodeIdsWithDuplicatedAliasPath);
            var treeNodeWithPageTypeNotAssignedToSiteResults = GetTreeNodeTestResult("Tree nodes with page type not assigned to site", Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite);
            var documentNodesWithMissingTreeNodeResults = GetDocumentNodeTestResult("Document nodes with no tree node", Scripts.GetDocumentIdsWithMissingTreeNode);

            var workflowInconsistenciesResults = GetWorkflowInconsistencyResult();

            return CompileResults(
                treeNodeWithBadParentSiteResults,
                treeNodeWithBadParentNodeResults,
                treeNodeWithLevelInconsistencyAliasatPathTestResults,
                treeNodeWithLevelInconsistencyParentChildLevelTestResults,
                treeNodeWithMissingDocumentResults,
                treeNodeWithDuplicateAliasPathResults,
                treeNodeWithPageTypeNotAssignedToSiteResults,
                documentNodesWithMissingTreeNodeResults,
                workflowInconsistenciesResults
                );
        }

        private IEnumerable<string> CompareVersionHistoryItemsWithPublishedItems(IEnumerable<CmsVersionHistoryItem> versionHistoryItems, Dictionary<int, int> documentIdToForeignKeyMapping, IEnumerable<IDictionary<string, object>> coupledData, IEnumerable<CmsClassField> cmsClassFields)
        {
            var issues = new List<string>();
            var idColumnName = cmsClassFields.FirstOrDefault(x => x.IsIdColumn).Column;

            foreach (var versionHistoryItem in versionHistoryItems)
            {
                var foreignKey = documentIdToForeignKeyMapping[versionHistoryItem.DocumentID];
                var coupledDataItem = coupledData.FirstOrDefault(x => (int)x[idColumnName] == foreignKey);

                if (coupledDataItem != null)
                {
                    foreach (var cmsClassField in cmsClassFields)
                    {
                        var historyVersionValueRaw = versionHistoryItem.NodeXml.SelectSingleNode($"//{cmsClassField.Column}")?.InnerText ?? cmsClassField.DefaultValue;
                        var coupledDataItemValue = coupledDataItem[cmsClassField.Column];
                        var bothNull = historyVersionValueRaw == null && coupledDataItemValue == null;
                        var bothMatch = bothNull || ItemValuesMatch(cmsClassField.ColumnType, historyVersionValueRaw, coupledDataItemValue);

                        if (!bothMatch)
                        {
                            issues.Add($"Document {versionHistoryItem.DocumentID} version history data ({historyVersionValueRaw}) didn't match published data ({coupledDataItemValue})");
                        }
                    }
                }
            }

            return issues;
        }

        private static bool ItemValuesMatch(string columnType, string xmlValue, object columnValue)
        {
            if (xmlValue == null || columnValue == null)
            {
                return false;
            }

            switch (columnType)
            {
                case FieldColumnTypes.Boolean:
                    return bool.Parse(xmlValue) == (bool)columnValue;
                case FieldColumnTypes.DateTime:
                    var versionHistoryValue = DateTimeOffset.Parse(xmlValue);
                    var columnValueAdjusted = new DateTimeOffset((DateTime)columnValue, versionHistoryValue.Offset);
                    return versionHistoryValue == columnValueAdjusted;
                //2019-06-06T11:37:22-04:00
                case FieldColumnTypes.Decimal:
                    return decimal.Parse(xmlValue) == (decimal)columnValue;
                default:
                    return xmlValue == columnValue.ToString();
            }
        }

        private ReportResults CompileResults(params ReportResults[] allReportResults)
        {
            var combinedResults = new ReportResults();

            combinedResults.Type = ReportResultsType.TableList;
            combinedResults.Status = ReportResultsStatus.Good;

            foreach (var reportResults in allReportResults)
            {
                var name = ((string)reportResults.Data.Name);
                // TODO: Make this WAY better
                ((IDictionary<string, object>)combinedResults.Data).Add(reportResults.Data.Name, reportResults.Data);
                if (reportResults.Status == ReportResultsStatus.Error)
                {
                    combinedResults.Summary += $"{name} found. ";
                    combinedResults.Status = ReportResultsStatus.Error;
                }
            }

            if (combinedResults.Status == ReportResultsStatus.Good)
            {
                combinedResults.Summary = "No content tree consistency issues found.";
            }

            return combinedResults;
        }

        private List<KeyValuePair<int, CmsClassField>> GetCmsClassFieldsFromXml(IEnumerable<CmsClassItem> CmsClassItems)
        {
            var result = new List<KeyValuePair<int, CmsClassField>>();
            foreach (var cmsClassItem in CmsClassItems)
            {
                var fields = cmsClassItem.ClassFormDefinitionXml.SelectNodes("/form/field");

                foreach (XmlNode field in fields)
                {
                    var isIdColumnRaw = field.Attributes["isPK"]?.Value;
                    var isIdColumn = !string.IsNullOrWhiteSpace(isIdColumnRaw) ? bool.Parse(isIdColumnRaw) : false;

                    var classIdClassFieldPair = new KeyValuePair<int, CmsClassField>(cmsClassItem.ClassID, new CmsClassField
                    {
                        Caption = field.SelectSingleNode("/properties/fieldcaption")?.Value,
                        Column = field.Attributes["column"].Value,
                        ColumnType = field.Attributes["columntype"].Value,
                        DefaultValue = field.SelectSingleNode("/properties/defaultvalue")?.InnerText,
                        IsIdColumn = isIdColumn
                    });

                    result.Add(classIdClassFieldPair);
                }
            }

            return result;
        }

        private IEnumerable<CmsClassItem> GetCmsClassItems(IEnumerable<CmsVersionHistoryItem> versionHistoryItems)
        {
            var cmsClassIds = versionHistoryItems.Select(vhi => vhi.VersionClassID);
            return _databaseService.ExecuteSqlFromFile<CmsClassItem>(Scripts.GetCmsClassItems, new { IDs = cmsClassIds.ToArray() });
        }

        private ReportResults GetDocumentNodeTestResult(string name, string script)
        {
            return GetTestResult<CmsDocumentNode>(name, script, Scripts.GetDocumentNodeDetails);
        }

        private IEnumerable<IDictionary<string, object>> GetCoupledData(string tableName, string idColumnName, IEnumerable<int> Ids)
        {
            var replacements = new Dictionary<string, string>();
            replacements.Add("TableName", tableName);
            replacements.Add("IdColumnName", idColumnName);

            return _databaseService.ExecuteSqlFromFileWithReplacements(Scripts.GetCmsDocumentCoupledDataItems, replacements, new { IDs = Ids.ToArray() });
        }

        private ReportResults GetTestResult<T>(string name, string script, string getDetailsScript)
        {
            var nodeIds = _databaseService.ExecuteSqlFromFile<int>(script);
            var details = _databaseService.ExecuteSqlFromFile<T>(getDetailsScript, new { IDs = nodeIds.ToArray() });

            var data = new TableResult<T>
            {
                Name = name,
                Rows = details
            };

            return new ReportResults
            {
                Data = data,
                Status = data.Rows.Count() > 0 ? ReportResultsStatus.Error : ReportResultsStatus.Good,
                Summary = string.Empty,
                Type = ReportResultsType.Table,
            };
        }

        private ReportResults GetTreeNodeTestResult(string name, string script)
        {
            return GetTestResult<CmsTreeNode>(name, script, Scripts.GetTreeNodeDetails);
        }

        private IEnumerable<CmsVersionHistoryItem> GetVersionHistoryItems()
        {
            var latestVersionHistoryIds = _databaseService.ExecuteSqlFromFile<int>(Scripts.GetLatestVersionHistoryIdForAllDocuments);
            return _databaseService.ExecuteSqlFromFile<CmsVersionHistoryItem>(Scripts.GetVersionHistoryDetails, new { IDs = latestVersionHistoryIds.ToArray() });
        }

        private ReportResults GetWorkflowInconsistencyResult()
        {
            var versionHistoryItems = GetVersionHistoryItems();
            var versionHistoryDocumentIdToForeignKeyMapping = GetDocumentIdToForeignKeyMapping(versionHistoryItems);

            var cmsClassItems = GetCmsClassItems(versionHistoryItems);
            var allCmsClassFields = GetCmsClassFieldsFromXml(cmsClassItems);

            var comparisonResults = new List<string>();
            foreach (var cmsClass in cmsClassItems)
            {
                var cmsClassVersionHistoryItems = versionHistoryItems.Where(vhi => vhi.VersionClassID == cmsClass.ClassID);
                var cmsClassFields = allCmsClassFields.Where(x => x.Key == cmsClass.ClassID).Select(x => x.Value);
                var cmsClassIdColumn = cmsClassFields.Where(x => x.IsIdColumn).Select(x => x.Column).FirstOrDefault();
                var documentIds = cmsClassVersionHistoryItems.Select(x => x.DocumentID);
                var documentNodes = _databaseService.ExecuteSqlFromFile<CmsDocumentNode>(Scripts.GetDocumentNodeDetails, new { IDs = documentIds.ToArray() });
                var coupledDataIds = documentNodes.Select(x => x.DocumentForeignKeyValue);
                var coupledData = GetCoupledData(cmsClass.ClassTableName, cmsClassIdColumn, coupledDataIds);
                comparisonResults.Concat(CompareVersionHistoryItemsWithPublishedItems(versionHistoryItems, versionHistoryDocumentIdToForeignKeyMapping, coupledData, cmsClassFields));
            }

            // TODO: Aggregate any issues
            return new ReportResults() {
                Data = comparisonResults,
                Type = ReportResultsType.StringList
            };
        }

        private Dictionary<int, int> GetDocumentIdToForeignKeyMapping(IEnumerable<CmsVersionHistoryItem> versionHistoryItems)
        {
            var mapping = new Dictionary<int, int>();

            foreach (var versionHistoryItem in versionHistoryItems)
            {
                var foreignKeyRaw = versionHistoryItem.NodeXml.SelectSingleNode("//DocumentForeignKeyValue")?.InnerText;
                int foreignKey;

                if (int.TryParse(foreignKeyRaw, out foreignKey))
                {
                    mapping.Add(versionHistoryItem.DocumentID, foreignKey);
                }
            }

            return mapping;
        }
    }
}
