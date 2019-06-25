using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseConsistencyCheck;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;
using NUnit.Framework;
using System.Data;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseConsistencyCheckTests
    {
        private Instance _mockInstance;
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IInstanceService> _mockInstanceService;
        private Report _mockReport;

        public DatabaseConsistencyCheckTests(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object);
        }

        [Test]
        public void Should_ReturnGoodStatus_When_ResultsEmpty()
        {
            // Arrange
            var emptyResult = new DataTable();
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(emptyResult);
#pragma warning restore 0618
            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            //Assert
            Assert.That(results.Status == ReportResultsStatus.Good);
        }

        [Test]
        public void Should_ReturnErrorStatus_When_ResultsNotEmpty()
        {
            // Arrange
            var result = new DataTable();
            result.Columns.Add("TestColumn");
            result.Rows.Add("value");

# pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(result);
# pragma warning restore 0618 

            // Act
            var results = _mockReport.GetResults(_mockInstance.Guid);

            //Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        private void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
        }
    }
}
