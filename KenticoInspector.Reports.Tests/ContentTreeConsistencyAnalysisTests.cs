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
            var badCmsDocumentNodes = new List<CmsDocumentNode>() {
                new CmsDocumentNode { DocumentID = 100, DocumentName = "Bad 100", DocumentNamePath = "/bad-100", DocumentNodeID = 100 },
                new CmsDocumentNode { DocumentID = 150, DocumentName = "Bad 150", DocumentNamePath = "/bad-150", DocumentNodeID = 150 }
            };
            SetupCmsDocumentNodeIdAndDetailsDatabaseQueries(Scripts.GetDocumentIdsWithMissingTreeNode, badCmsDocumentNodes);

            var goodCmsTreeNodes = new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithBadParentNodeId, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithBadParentSiteId, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithDuplicatedAliasPath, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithMissingDocument, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite, goodCmsTreeNodes);

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnGoodResult_When_DatabaseIsClean()
        {
            // Arrange
            var goodCmsDocumentNodes = new List<CmsDocumentNode>();
            SetupCmsDocumentNodeIdAndDetailsDatabaseQueries(Scripts.GetDocumentIdsWithMissingTreeNode, goodCmsDocumentNodes);

            var goodCmsTreeNodes = new List<CmsTreeNode>();
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithBadParentNodeId, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithBadParentSiteId, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithDuplicatedAliasPath, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithMissingDocument, goodCmsTreeNodes);
            SetupCmsTreeNodeIdAndDetailsDatabaseQueries(Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite, goodCmsTreeNodes);

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
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
