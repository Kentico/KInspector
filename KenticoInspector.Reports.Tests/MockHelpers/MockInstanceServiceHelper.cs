using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.Tests.MockHelpers
{
    public static class MockInstanceServiceHelper
    {
        public static Mock<IInstanceService> SetupInstanceService(Instance instance, InstanceDetails instanceDetails)
        {
            var mockInstanceService = new Mock<IInstanceService>(MockBehavior.Strict);
            mockInstanceService.Setup(p => p.GetInstance(instance.Guid)).Returns(instance);
            mockInstanceService.Setup(p => p.GetInstanceDetails(instance)).Returns(instanceDetails);
            return mockInstanceService;
        }
    }
}
