using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using Moq;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockInstanceServiceHelper
    {
        public static Mock<IInstanceService> SetupInstanceService(Instance instance, InstanceDetails instanceDetails)
        {
            var mockInstanceService = new Mock<IInstanceService>(MockBehavior.Strict);
            mockInstanceService.Setup(p => p.CurrentInstance).Returns(instance);
            mockInstanceService.Setup(p => p.GetInstance(instance.Guid)).Returns(instance);
            mockInstanceService.Setup(p => p.GetInstanceDetails(instance)).Returns(instanceDetails);
            return mockInstanceService;
        }
    }
}