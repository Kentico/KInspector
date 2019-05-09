using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseConsistencyCheck;
using KenticoInspector.Reports.Tests.MockHelpers;
using Moq;
using NUnit.Framework;
using System.Data;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseTableSizeAnalysisTest
    {
        private Instance _mockInstance;
        private InstanceDetails _mockInstanceDetails;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IInstanceService> _mockInstanceService;
        private Report _mockReport;

        public DatabaseTableSizeAnalysisTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object);
        }

        [Test]
        public void Should_ReturnTop25Databases_In_Size()
        {

        }

        private void InitializeCommonMocks(int majorVersion)
        {

        }
    }
}
