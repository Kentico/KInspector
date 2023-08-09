using KenticoInspector.Core.Services.Interfaces;

using Moq;

namespace KenticoInspector.Modules.Tests.Helpers
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