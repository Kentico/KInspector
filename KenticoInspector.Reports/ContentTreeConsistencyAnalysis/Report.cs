using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>()
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

        private ReportResults CompileResults(params ReportResults[] allReportResults)
        {
            var combinedResults = new ReportResults();

            combinedResults.Type = ReportResultsType.TableList;
            combinedResults.Status = ReportResultsStatus.Good;

            var summaryBuilder = new StringBuilder();
            foreach (var reportResults in allReportResults)
            {
                var name = ((string)reportResults.Data.Name);
                ((IDictionary<string, object>)combinedResults.Data).Add(reportResults.Data.Name, reportResults.Data);
                if (reportResults.Status == ReportResultsStatus.Error)
                {
                    summaryBuilder.Append(Metadata.Terms.NameFound.With(new { name }));
                    combinedResults.Status = ReportResultsStatus.Error;
                }
            }

            combinedResults.Summary = summaryBuilder.ToString();
            if (combinedResults.Status == ReportResultsStatus.Good)
            {
                combinedResults.Summary = Metadata.Terms.NoContentTreeConsistencyIssuesFound;
            }

            return combinedResults;
        }

        private IEnumerable<CmsClassItem> GetCmsClassItems(IEnumerable<CmsVersionHistoryItem> versionHistoryItems)
        {
            var cmsClassIds = versionHistoryItems.Select(vhi => vhi.VersionClassID);
            return databaseService.ExecuteSqlFromFile<CmsClassItem>(Scripts.GetCmsClassItems, new { IDs = cmsClassIds.ToArray() });
        }

        private IEnumerable<IDictionary<string, object>> GetCoupledData(CmsClassItem cmsClassItem, IEnumerable<int> Ids)
        {
            var replacements = new CoupledDataScriptReplacements(cmsClassItem.ClassTableName, cmsClassItem.ClassIDColumn);
            return databaseService.ExecuteSqlFromFileGeneric(Scripts.GetCmsDocumentCoupledDataItems, replacements.Dictionary, new { IDs = Ids.ToArray() });
        }

        private ReportResults GetDocumentNodeTestResult(string name, string script)
        {
            return GetTestResult<CmsDocumentNode>(name, script, Scripts.GetDocumentNodeDetails);
        }

        private ReportResults GetTestResult<T>(string name, string script, string getDetailsScript)
        {
            var nodeIds = databaseService.ExecuteSqlFromFile<int>(script);
            var details = databaseService.ExecuteSqlFromFile<T>(getDetailsScript, new { IDs = nodeIds.ToArray() });

            var data = new TableResult<T>
            {
                Name = name,
                Rows = details
            };

            return new ReportResults
            {
                Data = data,
                Status = data.Rows.Any() ? ReportResultsStatus.Error : ReportResultsStatus.Good,
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
            var latestVersionHistoryIds = databaseService.ExecuteSqlFromFile<int>(Scripts.GetLatestVersionHistoryIdForAllDocuments);
            return databaseService.ExecuteSqlFromFile<CmsVersionHistoryItem>(Scripts.GetVersionHistoryDetails, new { IDs = latestVersionHistoryIds.ToArray() });
        }

        private ReportResults GetWorkflowInconsistencyResult()
        {
            var versionHistoryItems = GetVersionHistoryItems();
            var cmsClassItems = GetCmsClassItems(versionHistoryItems);
            var comparisonResults = new List<VersionHistoryMismatchResult>();
            foreach (var cmsClass in cmsClassItems)
            {
                var cmsClassVersionHistoryItems = versionHistoryItems.Where(vhi => vhi.VersionClassID == cmsClass.ClassID);
                var coupledDataIds = cmsClassVersionHistoryItems.Select(x => x.CoupledDataID);
                var coupledData = GetCoupledData(cmsClass, coupledDataIds);
                var classComparisionResults = CompareVersionHistoryItemsWithPublishedItems(versionHistoryItems, coupledData, cmsClass.ClassFields);
                comparisonResults.AddRange(classComparisionResults);
            }

            var data = new TableResult<VersionHistoryMismatchResult>
            {
                Name = Metadata.Terms.WorkflowInconsistencies,
                Rows = comparisonResults
            };

            return new ReportResults
            {
                Data = data,
                Status = data.Rows.Any() ? ReportResultsStatus.Error : ReportResultsStatus.Good,
                Summary = string.Empty,
                Type = ReportResultsType.Table,
            };
        }
    }
}