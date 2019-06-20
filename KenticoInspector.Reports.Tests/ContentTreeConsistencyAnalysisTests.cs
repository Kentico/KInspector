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
            List<CmsVersionHistoryItem> versionHistoryItems = null,
            List<CmsClassItem> versionHistoryCmsClassItems = null,
            List<CmsDocumentNode> versionHistoryTreeNodes = null
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

            versionHistoryItems = versionHistoryItems ?? new List<CmsVersionHistoryItem>();
            SetupCmsVersionHistoryQueries(versionHistoryItems, Scripts.GetLatestVersionHistoryIdForAllDocuments);

            versionHistoryCmsClassItems = versionHistoryCmsClassItems ?? new List<CmsClassItem>();
            SetupCmsClassItemsQueries(versionHistoryCmsClassItems);

            // TODO: Setup GetDocumentDetails for ALL classes
            versionHistoryTreeNodes = versionHistoryTreeNodes ?? new List<CmsDocumentNode>();
            foreach (var classItem in versionHistoryCmsClassItems)
            {
                //var classDocuments = versionHistoryTreeNodes.Where(x => x. == classItem.ClassID);
                //SetupCmsDocumentNodeQueries(classDocuments)
            }
            
            // TODO: Setup document coupled data 
            //_databaseService.ExecuteSqlFromFileWithReplacements(Scripts.GetCmsDocumentCoupledDataItems, replacements, new { IDs = Ids.ToArray() });
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
    }
}
