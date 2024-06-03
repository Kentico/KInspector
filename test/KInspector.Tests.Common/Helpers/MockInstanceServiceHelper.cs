using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

using Moq;

namespace KInspector.Tests.Common.Helpers
{
    public static class MockInstanceServiceHelper
    {
        public static Mock<IInstanceService> SetupInstanceService(Instance instance, InstanceDetails instanceDetails)
        {
            var mockInstanceService = new Mock<IInstanceService>(MockBehavior.Strict);
            mockInstanceService.Setup(p => p.GetInstanceDetails(instance)).Returns(instanceDetails);

            return mockInstanceService;
        }
    }
}