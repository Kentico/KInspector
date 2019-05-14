using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.Tests.MockHelpers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

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

            MockAllScriptsClean();

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Information.ToString());
        }

        private void MockAllScriptsClean()
        {
            var CleanIdList = new List<int>();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetDocumentIdsWithMissingTreeNode, null))
                .Returns(CleanIdList);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetTreeNodeIdsWithBadParentNodeId, null))
                .Returns(CleanIdList);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetTreeNodeIdsWithBadParentSiteId, null))
                .Returns(CleanIdList);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetTreeNodeIdsWithDuplicatedAliasPath, null))
                .Returns(CleanIdList);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest, null))
                .Returns(CleanIdList);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest, null))
                .Returns(CleanIdList);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetTreeNodeIdsWithMissingDocument, null))
                .Returns(CleanIdList);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<int>(Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite, null))
                .Returns(CleanIdList);

            var CleanCmsDocumentNodeList = new List<CmsDocumentNode>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsDocumentNode>(Scripts.GetDocumentNodeDetails, It.IsAny<object>()))
                .Returns(CleanCmsDocumentNodeList);

            var CleanCmsTreeNodeList = new List<CmsTreeNode>();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsTreeNode>(Scripts.GetTreeNodeDetails, It.IsAny<object>()))
                .Returns(CleanCmsTreeNodeList);
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
        }

        private void SetupExecuteSqlFromFile<T,U>(string script, U parameters, IEnumerable<T> returns)
        {
            _mockDatabaseService.Setup(p => p.ExecuteSqlFromFile<T>(script, parameters))
                .Returns(returns);
        }
    }
}
