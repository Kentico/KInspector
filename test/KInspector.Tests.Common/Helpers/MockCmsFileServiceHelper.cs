using KInspector.Core.Services.Interfaces;

using Moq;

namespace KInspector.Tests.Common.Helpers
{
    public static class MockCmsFileServiceHelper
    {
        public static Mock<ICmsFileService> SetupMockCmsFileService()
        {
            var mockCmsFileService = new Mock<ICmsFileService>(MockBehavior.Strict);

            return mockCmsFileService;
        }
    }
}