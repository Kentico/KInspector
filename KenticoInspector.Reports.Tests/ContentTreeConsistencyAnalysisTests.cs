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
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithBadParent()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodeIdsWithBadParentNodeId: GetBadTreeNodes());

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
            List<CmsTreeNode> treeNodeIdsWithBadParentNodeId = null,
            List<CmsTreeNode> treeNodeIdsWithBadParentSiteId = null,
            List<CmsTreeNode> treeNodeIdsWithDuplicatedAliasPath = null,
            List<CmsTreeNode> treeNodeIdsWithLevelMismatchByAliasPathTest = null,
            List<CmsTreeNode> treeNodeIdsWithLevelMismatchByNodeLevelTest = null,
            List<CmsTreeNode> treeNodeIdsWithMissingDocument = null,
            List<CmsTreeNode> treeNodeIdsWithPageTypeNotAssignedToSite = null
            )
        {
            documentsWithMissingTreeNode = documentsWithMissingTreeNode ?? new List<CmsDocumentNode>();
            SetupCmsDocumentNodeIdAndDetailsDatabaseQueries(Scripts.GetDocumentIdsWithMissingTreeNode, documentsWithMissingTreeNode);

            treeNodeIdsWithBadParentNodeId = treeNodeIdsWithBadParentNodeId ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithBadParentNodeId, treeNodeIdsWithBadParentNodeId);

            treeNodeIdsWithBadParentSiteId = treeNodeIdsWithBadParentSiteId ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithBadParentSiteId, treeNodeIdsWithBadParentSiteId);

            treeNodeIdsWithDuplicatedAliasPath = treeNodeIdsWithDuplicatedAliasPath ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithDuplicatedAliasPath, treeNodeIdsWithDuplicatedAliasPath);

            treeNodeIdsWithLevelMismatchByAliasPathTest = treeNodeIdsWithLevelMismatchByAliasPathTest ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest, treeNodeIdsWithLevelMismatchByAliasPathTest);

            treeNodeIdsWithLevelMismatchByNodeLevelTest = treeNodeIdsWithLevelMismatchByNodeLevelTest ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest, treeNodeIdsWithLevelMismatchByNodeLevelTest);

            treeNodeIdsWithMissingDocument = treeNodeIdsWithMissingDocument ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithMissingDocument, treeNodeIdsWithMissingDocument);

            treeNodeIdsWithPageTypeNotAssignedToSite = treeNodeIdsWithPageTypeNotAssignedToSite ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite, treeNodeIdsWithPageTypeNotAssignedToSite);

        }

        private void SetupCmsDocumentNodeIdAndDetailsDatabaseQueries(string idScript, IEnumerable<CmsDocumentNode> detailsValue = null)
        {
            if (detailsValue == null)
            {
                detailsValue = new List<CmsDocumentNode>();
            }

            var idValue = detailsValue.Select(x => x.DocumentID);
            _mockDatabaseService.SetupExecuteSqlFromFile(idScript, idValue);
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetDocumentNodeDetails, "IDs", idValue, detailsValue);
        }

        private void SetupCmsTreeNodeIdAndDetailsDatabaseQueries(string idScript, IEnumerable<CmsTreeNode> detailsValue = null)
        {
            if (detailsValue == null) {
                detailsValue = new List<CmsTreeNode>();
            }

            var idValue = detailsValue.Select(x => x.NodeID);
            _mockDatabaseService.SetupExecuteSqlFromFile(idScript, idValue);
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetTreeNodeDetails, "IDs", idValue, detailsValue);
        }
    }
}
