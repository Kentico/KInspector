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
        <p>Checks that CMS_Tree and CMS_Document tables are without any consistency issues.</p>
        ";

        public string Name => "Content Tree Consistency Analysis";

        public string ShortDescription => "Performs consistency analysis for content items in the content tree";

        public IList<string> Tags => new List<string>()
        {
            ReportTags.Health,
            ReportTags.Consistency
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            _databaseService.ConfigureForInstance(instance);

            var testResults = new Dictionary<string, IEnumerable<int>>();

            var treeNodeWithBadParentSiteResults = GetTreeNodeTestResult("Tree Nodes with Bad Parent Site", Scripts.GetTreeNodeIdsWithBadParentSiteId);
            var treeNodeWithBadParentNodeResults = GetTreeNodeTestResult("Tree Nodes with Bad Parent Node", Scripts.GetTreeNodeIdsWithBadParentNodeId);
            var treeNodeWithLevelInconsistencyAliasatPathTestResults = GetTreeNodeTestResult("Tree Nodes with Level Inconsistency (alias path test)", Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest);
            var treeNodeWithLevelInconsistencyParentChildLevelTestResults = GetTreeNodeTestResult("Tree Nodes with Level Inconsistency (parent/child level test)", Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest);
            var treeNodeWithMissingDocumentResults = GetTreeNodeTestResult("Tree Nodes with Missing Document", Scripts.GetTreeNodeIdsWithMissingDocument);

            var treeNodeDuplicateTestResult = GetTreeNodeDuplicateTestResult();

            // TODO: Build report for Documents with missing Tree Node
            //var documentIdsWithMissingTreeNode = _databaseService.ExecuteSqlFromFile<int>(Scripts.GetDocumentIdsWithMissingTreeNode);

            // TODO: Build report for Page Types Used without being assigned to Site
            //var pageTypeAssignmentResults = _databaseService.ExecuteSqlFromFile<PageTypeAssignmentResult>(Scripts.GetPageTypeAssignmentResults);

            return CompileResults(
                treeNodeWithBadParentSiteResults,
                treeNodeWithBadParentNodeResults,
                treeNodeWithLevelInconsistencyAliasatPathTestResults,
                treeNodeWithLevelInconsistencyParentChildLevelTestResults,
                treeNodeWithMissingDocumentResults,
                treeNodeDuplicateTestResult
                );
        }

        private ReportResults GetTreeNodeTestResult(string name, string script)
        {
            var IDs = _databaseService.ExecuteSqlFromFile<int>(script).ToArray();
            var nodeDetails = _databaseService.ExecuteSqlFromFile<CmsTreeNode>(Scripts.GetTreeNodeDetails, new { IDs });

            var data = new TableResult<CmsTreeNode>
            {
                Name = name,
                Rows = nodeDetails
            };

            return new ReportResults
            {
                Data = data,
                Status = ReportResultsStatus.Information.ToString(),
                Summary = string.Empty,
                Type = ReportResultsType.Table.ToString(),
            };
        }

        private ReportResults GetTreeNodeDuplicateTestResult()
        {
            var treeNodeDuplicateResults = _databaseService.ExecuteSqlFromFile<TreeNodeDuplicateResult>(Scripts.GetDuplicateTreeNodeIdPairs);

            var IDs = treeNodeDuplicateResults.SelectMany(x => new int[] { x.NodeID, x.DuplicateNodeID }).Distinct().ToArray();
            var nodeDetails = _databaseService.ExecuteSqlFromFile<CmsTreeNode>(Scripts.GetTreeNodeDetails, new { IDs });

            var treeNodeOriginals = new List<int>();
            var treeNodeDuplicates = new List<int>();
            foreach (var treeNodeDuplicateResult in treeNodeDuplicateResults)
            {
                if (!treeNodeDuplicates.Contains(treeNodeDuplicateResult.NodeID))
                {
                    treeNodeOriginals.Add(treeNodeDuplicateResult.NodeID);
                    treeNodeDuplicates.Add(treeNodeDuplicateResult.DuplicateNodeID);
                }
            }

            var rows = treeNodeDuplicateResults
                .Where(x => treeNodeOriginals.Contains(x.NodeID))
                .Select(x =>
                {
                    var originalNodeDetail = nodeDetails.First(y => y.NodeID == x.NodeID);
                    var duplicateNodeDetail = nodeDetails.First(y => y.NodeID == x.DuplicateNodeID);
                    return new
                    {
                        originalNodeDetail.NodeAliasPath,
                        OriginalNodeId = originalNodeDetail.NodeID,
                        DuplicateNodeId = duplicateNodeDetail.NodeID
                    };
                });


            var data = new TableResult<dynamic>
            {
                Name = "Tree Node Duplicates",
                Rows = rows
            };

            return new ReportResults
            {
                Data = data,
                Status = ReportResultsStatus.Information.ToString(),
                Summary = string.Empty,
                Type = ReportResultsType.Table.ToString(),
            };
        }

        private ReportResults CompileResults(params ReportResults[] allReportResults)
        {
            var combinedResults = new ReportResults();

            combinedResults.Type = ReportResultsType.TableList.ToString();
            combinedResults.Status = ReportResultsStatus.Information.ToString();

            foreach (var reportResults in allReportResults)
            {
                var name = ((string)reportResults.Data.Name);
                combinedResults.Summary += reportResults.Summary;
                // TODO: Make this WAY better
                ((IDictionary<string, object>)combinedResults.Data).Add(reportResults.Data.Name, reportResults.Data);
                // TODO: logic to determine final status
            }

            return combinedResults;
        }
    }
}
