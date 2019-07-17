using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.Tests.Helpers;
using Moq;

namespace KenticoInspector.Reports.Tests
{
    public abstract class ReportTest
    {
        protected Instance _mockInstance;
        protected InstanceDetails _mockInstanceDetails;
        protected Mock<IDatabaseService> _mockDatabaseService;
        protected Mock<IInstanceService> _mockInstanceService;

        public ReportTest(int majorVersion)
        {
            InitializeCommonMocks(majorVersion);
        }

        protected virtual void InitializeCommonMocks(int majorVersion)
        {
            _mockInstance = MockInstances.Get(majorVersion);
            _mockInstanceDetails = MockInstanceDetails.Get(majorVersion, _mockInstance);
            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(_mockInstance, _mockInstanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(_mockInstance);
        }
    }
}
