using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

using Moq;

namespace KInspector.Tests.Common.Helpers
{
    public static class MockConfigServiceHelper
    {
        public static Mock<IConfigService> SetupMockConfigService(Instance? instance)
        {
            var mockConfigService = new Mock<IConfigService>(MockBehavior.Strict);
            mockConfigService.Setup(p => p.GetCurrentInstance()).Returns(instance);

            return mockConfigService;
        }
    }
}