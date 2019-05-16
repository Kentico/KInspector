using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            
            return CompileResults(
                treeNodeWithBadParentSiteResults,
                treeNodeWithBadParentNodeResults,
                treeNodeWithLevelInconsistencyAliasatPathTestResults,
                treeNodeWithLevelInconsistencyParentChildLevelTestResults,
                treeNodeWithMissingDocumentResults,
                treeNodeWithDuplicateAliasPathResults,
                treeNodeWithPageTypeNotAssignedToSiteResults,
                documentNodesWithMissingTreeNodeResults
                );
        }

        private ReportResults GetTreeNodeTestResult(string name, string script)
        {
            return GetTestResult<CmsTreeNode>(name, script, Scripts.GetTreeNodeDetails);
        }

        private ReportResults GetDocumentNodeTestResult(string name, string script)
        {
            return GetTestResult<CmsDocumentNode>(name, script, Scripts.GetDocumentNodeDetails);
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

        private ReportResults CompileResults(params ReportResults[] allReportResults)
        {
            var combinedResults = new ReportResults();

            combinedResults.Type = ReportResultsType.TableList;
            combinedResults.Status = ReportResultsStatus.Good;

            foreach (var reportResults in allReportResults)
            {
                var name = ((string)reportResults.Data.Name);
                combinedResults.Summary += reportResults.Summary;
                // TODO: Make this WAY better
                ((IDictionary<string, object>)combinedResults.Data).Add(reportResults.Data.Name, reportResults.Data);
                // TODO: logic to determine final status
                if (reportResults.Status == ReportResultsStatus.Error) {
                    combinedResults.Status = ReportResultsStatus.Error;
                }
            }

            return combinedResults;
        }
    }
}
