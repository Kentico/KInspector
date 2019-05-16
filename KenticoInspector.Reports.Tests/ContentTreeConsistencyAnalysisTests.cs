using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;
using NUnit.Framework;
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
        public void Should_ReturnCleanResult_When_DatabaseIsClean()
        {
            // Arrange

            MockScriptsClean();

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreDocumentsWithMissingTreeNode()
        {
            // Arrange
            var badCmsDocumentNodes = new List<CmsDocumentNode>() {
                new CmsDocumentNode { DocumentID = 100, DocumentName = "Bad 100", DocumentNamePath = "/bad-100", DocumentNodeID = 100 },
                new CmsDocumentNode { DocumentID = 150, DocumentName = "Bad 150", DocumentNamePath = "/bad-150", DocumentNodeID = 150 }
            };

            var badIds = badCmsDocumentNodes.Select(x => x.DocumentID);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetDocumentIdsWithMissingTreeNode, null))
                .Returns(badIds);

            SetupExecuteSqlFromFileWithListParameter(
                Scripts.GetDocumentNodeDetails,
                "IDs",
                badIds,
                badCmsDocumentNodes
            );

            MockScriptsClean(new string[] {
                Scripts.GetTreeNodeIdsWithBadParentNodeId,
                Scripts.GetTreeNodeIdsWithBadParentSiteId,
                Scripts.GetTreeNodeIdsWithDuplicatedAliasPath,
                Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest,
                Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest,
                Scripts.GetTreeNodeIdsWithMissingDocument,
                Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite
            });

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        private void MockScriptsClean(string[] idScripts = null)
        {
            idScripts = idScripts ?? new string[] {
                Scripts.GetDocumentIdsWithMissingTreeNode,
                Scripts.GetTreeNodeIdsWithBadParentNodeId,
                Scripts.GetTreeNodeIdsWithBadParentSiteId,
                Scripts.GetTreeNodeIdsWithDuplicatedAliasPath,
                Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest,
                Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest,
                Scripts.GetTreeNodeIdsWithMissingDocument,
                Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite
            };

            var CleanIdList = new List<int>();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(It.IsIn<string>(idScripts), null))
                .Returns(CleanIdList);

            var CleanCmsDocumentNodeList = new List<CmsDocumentNode>();
            SetupExecuteSqlFromFileWithListParameter(
                Scripts.GetDocumentNodeDetails,
                "IDs",
                CleanIdList,
                CleanCmsDocumentNodeList
            );

            var CleanCmsTreeNodeList = new List<CmsTreeNode>();
            SetupExecuteSqlFromFileWithListParameter(
                Scripts.GetTreeNodeDetails,
                "IDs",
                CleanIdList,
                CleanCmsTreeNodeList
            );
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
        }

        private void SetupExecuteSqlFromFileWithListParameter<T, U>(string script, string parameterPropertyName, IEnumerable<U> parameterPropertyValue, IEnumerable<T> returnValue)
        {
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<T>(script, It.Is<object>(actual => ObjectHelpers.ObjectPropertyValueEqualsExpectedValue(actual, parameterPropertyName, parameterPropertyValue))))
                .Returns(returnValue);
        }
    }
}
