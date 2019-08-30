using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Health,
            ReportTags.Consistency
        };

        public override ReportResults GetResults()
        {
            var treeNodeWithBadParentSiteResults = GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithABadParentSite, Scripts.GetTreeNodeIdsWithBadParentSiteId);
            var treeNodeWithBadParentNodeResults = GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithABadParentNode, Scripts.GetTreeNodeIdsWithBadParentNodeId);
            var treeNodeWithLevelInconsistencyAliasatPathTestResults = GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithLevelInconsistencyAliasPath, Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest);
            var treeNodeWithLevelInconsistencyParentChildLevelTestResults = GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithLevelInconsistencyParent, Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest);
            var treeNodeWithMissingDocumentResults = GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithNoDocumentNode, Scripts.GetTreeNodeIdsWithMissingDocument);
            var treeNodeWithDuplicateAliasPathResults = GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithDuplicatedAliasPath, Scripts.GetTreeNodeIdsWithDuplicatedAliasPath);
            var treeNodeWithPageTypeNotAssignedToSiteResults = GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithPageTypeNotAssignedToSite, Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite);
            var documentNodesWithMissingTreeNodeResults = GetDocumentNodeTestResult(Metadata.Terms.DocumentNodesWithNoTreeNode, Scripts.GetDocumentIdsWithMissingTreeNode);

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

        private ConsistencyResult GetTreeNodeTestResult(string tableName, string sqlScriptRelativeFilePath)
        {
            return GetTestResult<CmsTreeNode>(tableName, sqlScriptRelativeFilePath, Scripts.GetTreeNodeDetails);
        }

        private ConsistencyResult GetDocumentNodeTestResult(string name, string script)
        {
            return GetTestResult<CmsDocumentNode>(name, script, Scripts.GetDocumentNodeDetails);
        }

        private ConsistencyResult GetTestResult<T>(string name, string script, string getDetailsScript)
        {
            var nodeIds = databaseService.ExecuteSqlFromFile<int>(script);
            var details = databaseService.ExecuteSqlFromFile<T>(getDetailsScript, new { IDs = nodeIds.ToArray() });

            var data = details.AsResult(name);

            return new ConsistencyResult(
                data.Rows.Count() > 0 ? ReportResultsStatus.Error : ReportResultsStatus.Good,
                name,
                data
            );
        }

        private ConsistencyResult GetWorkflowInconsistencyResult()
        {
            var versionHistoryItems = GetVersionHistoryItems();

            var classItems = GetCmsClassItems(versionHistoryItems);

            // TODO: Find a use for this information
            // var allDocumentNodeIds = versionHistoryItems.Select(x => x.DocumentID);
            // var allDocumentNodes = _databaseService.ExecuteSqlFromFile<CmsDocumentNode>(Scripts.GetDocumentNodeDetails, new { IDs = allDocumentNodeIds.ToArray() });

            var comparisonResults = new List<VersionHistoryMismatchResult>();

            foreach (var cmsClass in classItems)
            {
                var classVersionHistoryItems = versionHistoryItems
                    .Where(versionHistoryItem => versionHistoryItem.VersionClassID == cmsClass.ClassID);

                var coupledDataIds = classVersionHistoryItems
                    .Select(classVersionHistoryItem => classVersionHistoryItem.CoupledDataID);

                var coupledData = GetCoupledData(cmsClass, coupledDataIds);

                var classComparisionResults = CompareVersionHistoryItemsWithPublishedItems(versionHistoryItems, coupledData, cmsClass.ClassFields);

                comparisonResults.AddRange(classComparisionResults);
            }

            var status = comparisonResults.Count() > 0 ? ReportResultsStatus.Error : ReportResultsStatus.Good;

            var tableName = Metadata.Terms.WorkflowInconsistencies;

            var data = comparisonResults.AsResult(tableName);

            return new ConsistencyResult(
                status,
                tableName,
                data
            );
        }

        private IEnumerable<CmsVersionHistoryItem> GetVersionHistoryItems()
        {
            var latestVersionHistoryIds = databaseService.ExecuteSqlFromFile<int>(Scripts.GetLatestVersionHistoryIdForAllDocuments);

            return databaseService.ExecuteSqlFromFile<CmsVersionHistoryItem>(Scripts.GetVersionHistoryDetails, new { IDs = latestVersionHistoryIds });
        }

        private IEnumerable<CmsClassItem> GetCmsClassItems(IEnumerable<CmsVersionHistoryItem> versionHistoryItems)
        {
            var cmsClassIds = versionHistoryItems.Select(vhi => vhi.VersionClassID);
            return databaseService.ExecuteSqlFromFile<CmsClassItem>(Scripts.GetCmsClassItems, new { IDs = cmsClassIds.ToArray() });
        }

        private IEnumerable<IDictionary<string, object>> GetCoupledData(CmsClassItem cmsClassItem, IEnumerable<int> Ids)
        {
            var replacements = new CoupledDataScriptReplacements(cmsClassItem.ClassTableName, cmsClassItem.ClassIDColumn);
            return databaseService.ExecuteSqlFromFileGeneric(Scripts.GetCmsDocumentCoupledDataItems, replacements.Dictionary, new { IDs = Ids });
        }

        private IEnumerable<VersionHistoryMismatchResult> CompareVersionHistoryItemsWithPublishedItems(IEnumerable<CmsVersionHistoryItem> versionHistoryItems, IEnumerable<IDictionary<string, object>> coupledData, IEnumerable<CmsClassField> cmsClassFields)
        {
            var issues = new List<VersionHistoryMismatchResult>();
            var idColumnName = cmsClassFields.FirstOrDefault(x => x.IsIdColumn).Column;

            foreach (var versionHistoryItem in versionHistoryItems)
            {
                var coupledDataItem = coupledData.FirstOrDefault(x => (int)x[idColumnName] == versionHistoryItem.CoupledDataID);

                if (coupledDataItem != null)
                {
                    foreach (var cmsClassField in cmsClassFields)
                    {
                        var historyVersionValueRaw = versionHistoryItem.NodeXml.SelectSingleNode($"//{cmsClassField.Column}")?.InnerText ?? cmsClassField.DefaultValue;
                        var coupledDataItemValue = coupledDataItem[cmsClassField.Column];
                        var columnName = cmsClassField.Caption ?? cmsClassField.Column;
                        var versionHistoryMismatchResult = new VersionHistoryMismatchResult(versionHistoryItem.DocumentID, columnName, cmsClassField.ColumnType, historyVersionValueRaw, coupledDataItemValue);

                        if (!versionHistoryMismatchResult.FieldValuesMatch)
                        {
                            issues.Add(versionHistoryMismatchResult);
                        }
                    }
                }
            }

            return issues;
        }

        private ReportResults CompileResults(params ConsistencyResult[] allTableResults)
        {
            var combinedResults = new ReportResults(ReportResultsStatus.Good);

            foreach (var reportResult in allTableResults)
            {
                var name = reportResult.TableName;

                if (reportResult.Status == ReportResultsStatus.Error)
                {
                    combinedResults.Data.Add(reportResult.Data);

                    var count = reportResult.Data.Rows.Count;

                    combinedResults.Summary += Metadata.Terms.NameFound.With(new { name, count });
                    combinedResults.Status = ReportResultsStatus.Error;
                }
            }

            if (combinedResults.Status == ReportResultsStatus.Good)
            {
                combinedResults.Summary = Metadata.Terms.NoContentTreeConsistencyIssuesFound;
            }

            return combinedResults;
        }
    }
}