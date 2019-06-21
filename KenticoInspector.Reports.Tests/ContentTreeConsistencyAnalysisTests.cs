using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ContentTreeConsistencyAnalysisTests
    {
        private Mock<IDatabaseService> _mockDatabaseService;
        private Instance _mockInstance;
        private InstanceDetails _mockInstanceDetails;
        private Mock<IInstanceService> _mockInstanceService;
        private Report _mockReport;

        public ContentTreeConsistencyAnalysisTests(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreDocumentsWithMissingTreeNode()
        {
            // Arrange
            SetupAllDatabaseQueries(documentsWithMissingTreeNode: GetBadDocumentNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithBadParentNode()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithBadParentNodeId: GetBadTreeNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithBadParentSite()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithBadParentSiteId: GetBadTreeNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithDuplicatedAliasPath()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithDuplicatedAliasPath: GetBadTreeNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithLevelMismatchByAliasPathTest()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithLevelMismatchByAliasPathTest: GetBadTreeNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithLevelMismatchByNodeLevelTest()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithLevelMismatchByNodeLevelTest: GetBadTreeNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithMissingDocument()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithMissingDocument: GetBadTreeNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithPageTypeNotAssignedToSite()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithPageTypeNotAssignedToSite: GetBadTreeNodes());

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreVersionHistoryErrors()
        {
            // Arrange
            SetupAllDatabaseQueries(isVersionHistoryDataSetClean: false);

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error, $"Status was '{results.Status}' instead of 'Error'");
            var resultsData = (IDictionary<string, object>)results.Data;
            var workflowData = (TableResult<VersionHistoryMismatchResult>)resultsData["Workflow Inconsistencies"];

            var rowCount = workflowData.Rows.Count();
            Assert.That(rowCount == 4, $"There were {rowCount} rows instead 4 as expected");
        }

        [Test]
        public void Should_ReturnGoodResult_When_DatabaseIsClean()
        {
            // Arrange
            SetupAllDatabaseQueries();

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);
        }

        private List<CmsDocumentNode> GetBadDocumentNodes()
        {
            return new List<CmsDocumentNode>() {
                new CmsDocumentNode { DocumentID = 100, DocumentName = "Bad 100", DocumentNamePath = "/bad-100", DocumentNodeID = 100 },
                new CmsDocumentNode { DocumentID = 150, DocumentName = "Bad 150", DocumentNamePath = "/bad-150", DocumentNodeID = 150 }
            };
        }

        private List<CmsTreeNode> GetBadTreeNodes()
        {
            return new List<CmsTreeNode>()
            {
                new CmsTreeNode { ClassDisplayName = "Bad Class", ClassName = "BadClass", NodeAliasPath = "/bad-1", NodeClassID = 1234, NodeID = 101, NodeLevel = 1, NodeName = "bad-1", NodeParentID = 0, NodeSiteID = 1 }
            };
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
        }

        private void SetupAllDatabaseQueries(
            List<CmsDocumentNode> documentsWithMissingTreeNode = null,
            List<CmsTreeNode> treeNodesWithBadParentNodeId = null,
            List<CmsTreeNode> treeNodesWithBadParentSiteId = null,
            List<CmsTreeNode> treeNodesWithDuplicatedAliasPath = null,
            List<CmsTreeNode> treeNodesWithLevelMismatchByAliasPathTest = null,
            List<CmsTreeNode> treeNodesWithLevelMismatchByNodeLevelTest = null,
            List<CmsTreeNode> treeNodesWithMissingDocument = null,
            List<CmsTreeNode> treeNodesWithPageTypeNotAssignedToSite = null,
            bool isVersionHistoryDataSetClean = true
            )
        {
            documentsWithMissingTreeNode = documentsWithMissingTreeNode ?? new List<CmsDocumentNode>();
            SetupCmsDocumentNodeQueries(documentsWithMissingTreeNode, Scripts.GetDocumentIdsWithMissingTreeNode);

            treeNodesWithBadParentNodeId = treeNodesWithBadParentNodeId ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithBadParentNodeId, Scripts.GetTreeNodeIdsWithBadParentNodeId);

            treeNodesWithBadParentSiteId = treeNodesWithBadParentSiteId ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithBadParentSiteId, Scripts.GetTreeNodeIdsWithBadParentSiteId);

            treeNodesWithDuplicatedAliasPath = treeNodesWithDuplicatedAliasPath ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithDuplicatedAliasPath, Scripts.GetTreeNodeIdsWithDuplicatedAliasPath);

            treeNodesWithLevelMismatchByAliasPathTest = treeNodesWithLevelMismatchByAliasPathTest ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithLevelMismatchByAliasPathTest, Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest);

            treeNodesWithLevelMismatchByNodeLevelTest = treeNodesWithLevelMismatchByNodeLevelTest ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithLevelMismatchByNodeLevelTest, Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest);

            treeNodesWithMissingDocument = treeNodesWithMissingDocument ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithMissingDocument, Scripts.GetTreeNodeIdsWithMissingDocument);

            treeNodesWithPageTypeNotAssignedToSite = treeNodesWithPageTypeNotAssignedToSite ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithPageTypeNotAssignedToSite, Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite);

            var versionHistoryDataSet = new VersionHistoryDataSet(isVersionHistoryDataSetClean);

            SetupCmsVersionHistoryQueries(versionHistoryDataSet.CmsVersionHistoryItems, Scripts.GetLatestVersionHistoryIdForAllDocuments);

            SetupCmsClassItemsQueries(versionHistoryDataSet.CmsClassItems);

            SetupVersionHistoryCoupledDataQueries(versionHistoryDataSet.CmsVersionHistoryItems, versionHistoryDataSet.CmsClassItems, versionHistoryDataSet.VersionHistoryCoupledData);
        }

        private void SetupVersionHistoryCoupledDataQueries(List<CmsVersionHistoryItem> versionHistoryItems, List<CmsClassItem> versionHistoryCmsClassItems, List<IDictionary<string, object>> versionHistoryCoupledData)
        {
            foreach (var cmsClassItem in versionHistoryCmsClassItems)
            {
                var coupledDataIds = versionHistoryItems
                    .Where(x => x.VersionClassID == cmsClassItem.ClassID)
                    .Select(x => x.CoupledDataID);
                var returnedItems = versionHistoryCoupledData
                    .Where(x => coupledDataIds.Contains((int)x[cmsClassItem.ClassIDColumn]));
                var literalReplacements = new CoupledDataScriptReplacements(cmsClassItem.ClassTableName, cmsClassItem.ClassIDColumn);

                _mockDatabaseService.SetupExecuteSqlFromFileGenericWithListParameter(
                    Scripts.GetCmsDocumentCoupledDataItems,
                    literalReplacements.Dictionary,
                    "IDs",
                    coupledDataIds,
                    returnedItems);
            }
        }

        private void SetupCmsClassItemsQueries(IEnumerable<CmsClassItem> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.ClassID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetCmsClassItems);
        }

        private void SetupCmsDocumentNodeQueries(IEnumerable<CmsDocumentNode> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.DocumentID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetDocumentNodeDetails);
        }

        private void SetupCmsTreeNodeQueries(IEnumerable<CmsTreeNode> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.NodeID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetTreeNodeDetails);
        }

        private void SetupCmsVersionHistoryQueries(IEnumerable<CmsVersionHistoryItem> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.VersionHistoryID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetVersionHistoryDetails);
        }

        private void SetupDetailsAndIdQueries<T>(IEnumerable<int> idValues, IEnumerable<T> returnedItems, string idScript, string detailsScript)
        {
            if (idValues == null)
            {
                throw new ArgumentNullException("idValues");
            }

            if (!string.IsNullOrWhiteSpace(idScript))
            {
                _mockDatabaseService.SetupExecuteSqlFromFile(idScript, idValues);
            }

            if (!string.IsNullOrWhiteSpace(detailsScript) && returnedItems != null)
            {
                _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(detailsScript, "IDs", idValues, returnedItems);
            }
        }

        private class VersionHistoryDataSet
        {
            public List<CmsVersionHistoryItem> CmsVersionHistoryItems { get; set; }
            public List<CmsClassItem> CmsClassItems { get; set; }
            public List<IDictionary<string, object>> VersionHistoryCoupledData { get; set; }

            public VersionHistoryDataSet(bool clean = true)
            {
                CmsClassItems = new List<CmsClassItem>();
                var classFormDefinitionXml5512 = new XmlDocument();
                classFormDefinitionXml5512.Load("TestData/classFormDefinitionXml_Clean_5512.xml");

                CmsClassItems.Add(new CmsClassItem
                {
                    ClassDisplayName = "",
                    ClassFormDefinitionXml = classFormDefinitionXml5512,
                    ClassID = 5512,
                    ClassName = "KIN.VersioningDataTest",
                    ClassTableName = "KIN_VersioningDataTest"
                });

                CmsVersionHistoryItems = new List<CmsVersionHistoryItem>();
                VersionHistoryCoupledData = new List<IDictionary<string, object>>();

                if (clean)
                {
                    var versionHistoryXml = new XmlDocument();
                    versionHistoryXml.Load("TestData/VersionHistoryItem_Clean_518.xml");

                    CmsVersionHistoryItems.Add(new CmsVersionHistoryItem
                    {
                        DocumentID = 518,
                        NodeXml = versionHistoryXml,
                        VersionClassID = 5512,
                        VersionHistoryID = 17,
                        WasPublishedFrom = DateTime.Parse("2019-06-06 11:58:49.2430968")
                    });

                    var coupledData = new Dictionary<string, object>();
                    coupledData.Add("VersioningDataTestID", 5);
                    coupledData.Add("BoolNoDefault", false);
                    coupledData.Add("BoolDefaultTrue", true);
                    coupledData.Add("BoolDefaultFalse", false);
                    coupledData.Add("DateTimeNoDefault", null);
                    coupledData.Add("DateTimeHardDefault", DateTime.Parse("2019-06-06 11:31:17.0000000"));
                    coupledData.Add("DateTimeMacroDefault", DateTime.Parse("2019-06-06 11:58:33.0000000"));
                    coupledData.Add("TextNoDefault", null);
                    coupledData.Add("TextHardDefault", "This is the default");
                    coupledData.Add("DecimalNoDefault", null);
                    coupledData.Add("DecimalHardDefault", 1.7500m);

                    VersionHistoryCoupledData.Add(coupledData);
                }
                else
                {
                    var versionHistoryXml = new XmlDocument();
                    versionHistoryXml.Load("TestData/VersionHistoryItem_Corrupt_519.xml");

                    CmsVersionHistoryItems.Add(new CmsVersionHistoryItem
                    {
                        DocumentID = 519,
                        NodeXml = versionHistoryXml,
                        VersionClassID = 5512,
                        VersionHistoryID = 18,
                        WasPublishedFrom = DateTime.Parse("2019-06-14 10:46:18.4493088")
                    });

                    var coupledData = new Dictionary<string, object>();
                    coupledData.Add("VersioningDataTestID", 6);
                    coupledData.Add("BoolNoDefault", true);
                    coupledData.Add("BoolDefaultTrue", false);
                    coupledData.Add("BoolDefaultFalse", false);
                    coupledData.Add("DateTimeNoDefault", DateTime.Parse("2019-06-14 10:54:35.0000000"));
                    coupledData.Add("DateTimeHardDefault", DateTime.Parse("2019-06-14 10:45:36.0000000"));
                    coupledData.Add("DateTimeMacroDefault", DateTime.Parse("2019-06-14 10:45:37.0000000"));
                    coupledData.Add("TextNoDefault", "Text 1 (corrupted)");
                    coupledData.Add("TextHardDefault", "Text 2");
                    coupledData.Add("DecimalNoDefault", 1.0150m);
                    coupledData.Add("DecimalHardDefault", 1.0200m);

                    VersionHistoryCoupledData.Add(coupledData);
                }
            }
        }
    }
}
