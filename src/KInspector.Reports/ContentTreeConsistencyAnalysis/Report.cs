using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.ContentTreeConsistencyAnalysis.Models;

using System.Text;

namespace KInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>()
        {
            ModuleTags.Health,
            ModuleTags.Consistency
        };

        public async override Task<ModuleResults> GetResults()
        {
            var treeNodeWithBadParentSiteResults = await GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithABadParentSite, Scripts.GetTreeNodeIdsWithBadParentSiteId);
            var treeNodeWithBadParentNodeResults = await GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithABadParentNode, Scripts.GetTreeNodeIdsWithBadParentNodeId);
            var treeNodeWithLevelInconsistencyAliasatPathTestResults = await GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithLevelInconsistencyAliasPath, Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest);
            var treeNodeWithLevelInconsistencyParentChildLevelTestResults = await GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithLevelInconsistencyParent, Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest);
            var treeNodeWithMissingDocumentResults = await GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithNoDocumentNode, Scripts.GetTreeNodeIdsWithMissingDocument);
            var treeNodeWithDuplicateAliasPathResults = await GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithDuplicatedAliasPath, Scripts.GetTreeNodeIdsWithDuplicatedAliasPath);
            var treeNodeWithPageTypeNotAssignedToSiteResults = await GetTreeNodeTestResult(Metadata.Terms.TreeNodesWithPageTypeNotAssignedToSite, Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite);
            var documentNodesWithMissingTreeNodeResults = await GetDocumentNodeTestResult(Metadata.Terms.DocumentNodesWithNoTreeNode, Scripts.GetDocumentIdsWithMissingTreeNode);
            var workflowInconsistenciesResults = await GetWorkflowInconsistencyResult();

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

        private static List<VersionHistoryMismatchResult> CompareVersionHistoryItemsWithPublishedItems(
            IEnumerable<CmsVersionHistoryItem> versionHistoryItems,
            IEnumerable<IDictionary<string, object>> coupledData,
            IEnumerable<CmsClassField> cmsClassFields)
        {
            var issues = new List<VersionHistoryMismatchResult>();
            var idColumnName = cmsClassFields.FirstOrDefault(x => x.IsIdColumn)?.Column ?? string.Empty;

            foreach (var versionHistoryItem in versionHistoryItems)
            {
                var coupledDataItem = coupledData.FirstOrDefault(x => (int)x[idColumnName] == versionHistoryItem.CoupledDataID);

                if (coupledDataItem is not null)
                {
                    foreach (var cmsClassField in cmsClassFields)
                    {
                        var historyVersionValueRaw = versionHistoryItem.NodeXml?.SelectSingleNode($"//{cmsClassField.Column}")?.InnerText ?? cmsClassField.DefaultValue;
                        var coupledDataItemValue = coupledDataItem[cmsClassField.Column ?? string.Empty];
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

        private ModuleResults CompileResults(params ModuleResults[] allModuleResults)
        {
            var combinedResults = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Good
            };

            var summaryBuilder = new StringBuilder();
            foreach (var ModuleResults in allModuleResults)
            {
                foreach (var table in ModuleResults.TableResults)
                {
                    var name = table.Name;
                    combinedResults.TableResults.Add(new TableResult
                    {
                        Name = name,
                        Rows = table.Rows
                    });

                    if (ModuleResults.Status == ResultsStatus.Error)
                    {
                        summaryBuilder.Append(Metadata.Terms.NameFound?.With(new { name }));
                        combinedResults.Status = ResultsStatus.Error;
                    }
                }
            }

            combinedResults.Summary = summaryBuilder.ToString();
            if (combinedResults.Status == ResultsStatus.Good)
            {
                combinedResults.Summary = Metadata.Terms.NoContentTreeConsistencyIssuesFound;
                combinedResults.Type = ResultsType.NoResults;
            }

            return combinedResults;
        }

        private Task<IEnumerable<CmsClassItem>> GetCmsClassItems(IEnumerable<CmsVersionHistoryItem> versionHistoryItems)
        {
            var cmsClassIds = versionHistoryItems.Select(vhi => vhi.VersionClassID);

            return databaseService.ExecuteSqlFromFile<CmsClassItem>(Scripts.GetCmsClassItems, new { IDs = cmsClassIds.ToArray() });
        }

        private Task<IEnumerable<IDictionary<string, object>>> GetCoupledData(CmsClassItem cmsClassItem, IEnumerable<int> Ids)
        {
            var replacements = new CoupledDataScriptReplacements(cmsClassItem.ClassTableName, cmsClassItem.ClassIDColumn);

            return databaseService.ExecuteSqlFromFileGeneric(Scripts.GetCmsDocumentCoupledDataItems, replacements.Dictionary, new { IDs = Ids.ToArray() });
        }

        private Task<ModuleResults> GetDocumentNodeTestResult(string name, string script) => GetTestResult<CmsDocumentNode>(name, script, Scripts.GetDocumentNodeDetails);

        private async Task<ModuleResults> GetTestResult<T>(string name, string script, string getDetailsScript) where T : class
        {
            var nodeIds = await databaseService.ExecuteSqlFromFile<int>(script);
            var details = await databaseService.ExecuteSqlFromFile<T>(getDetailsScript, new { IDs = nodeIds.ToArray() });
            var results = new ModuleResults
            {
                Status = details.Any() ? ResultsStatus.Error : ResultsStatus.Good,
                Summary = string.Empty,
                Type = details.Any() ? ResultsType.TableList : ResultsType.NoResults,
            };
            results.TableResults.Add(new TableResult
            {
                Name = name,
                Rows = details
            });

            return results;
        }

        private Task<ModuleResults> GetTreeNodeTestResult(string name, string script) => GetTestResult<CmsTreeNode>(name, script, Scripts.GetTreeNodeDetails);

        private async Task<IEnumerable<CmsVersionHistoryItem>> GetVersionHistoryItems()
        {
            var latestVersionHistoryIds = await databaseService.ExecuteSqlFromFile<int>(Scripts.GetLatestVersionHistoryIdForAllDocuments);

            return await databaseService.ExecuteSqlFromFile<CmsVersionHistoryItem>(Scripts.GetVersionHistoryDetails, new { IDs = latestVersionHistoryIds.ToArray() });
        }

        private async Task<ModuleResults> GetWorkflowInconsistencyResult()
        {
            var versionHistoryItems = await GetVersionHistoryItems();
            var cmsClassItems = await GetCmsClassItems(versionHistoryItems);
            var comparisonResults = new List<VersionHistoryMismatchResult>();
            foreach (var cmsClass in cmsClassItems)
            {
                var cmsClassVersionHistoryItems = versionHistoryItems.Where(vhi => vhi.VersionClassID == cmsClass.ClassID);
                var coupledDataIds = cmsClassVersionHistoryItems.Select(x => x.CoupledDataID).Where(x => x > 0);
                if (!coupledDataIds.Any())
                {
                    continue;
                }

                var coupledData = await GetCoupledData(cmsClass, coupledDataIds);
                var classComparisionResults = CompareVersionHistoryItemsWithPublishedItems(versionHistoryItems, coupledData, cmsClass.ClassFields);
                comparisonResults.AddRange(classComparisionResults);
            }

            if (comparisonResults.Any())
            {
                var results = new ModuleResults
                {
                    Summary = string.Empty,
                    Status = ResultsStatus.Error,
                    Type = ResultsType.TableList
                };
                results.TableResults.Add(new TableResult
                {
                    Name = Metadata.Terms.WorkflowInconsistencies,
                    Rows = comparisonResults
                });

                return results;
            }

            return new ModuleResults
            {
                Summary = string.Empty,
                Status = ResultsStatus.Good,
                Type = ResultsType.NoResults
            };
        }
    }
}