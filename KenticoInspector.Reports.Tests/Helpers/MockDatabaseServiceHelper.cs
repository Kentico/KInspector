using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockDatabaseServiceHelper
    {
        public static Mock<IDatabaseService> SetupMockDatabaseService(Instance instance)
        {
            var mockDatabaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            mockDatabaseService.Setup(p => p.ConfigureForInstance(instance));
            return mockDatabaseService;
        }
    }
}
