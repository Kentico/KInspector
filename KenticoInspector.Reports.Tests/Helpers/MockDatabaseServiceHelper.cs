using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using Moq;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockDatabaseServiceHelper
    {
        public static void SetupExecuteSqlFromFileWithListParameter<T, U>(
            this Mock<IDatabaseService> mockDatabaseService,
            string script,
            string parameterPropertyName,
            IEnumerable<U> parameterPropertyValue,
            IEnumerable<T> returnValue)
        {
            mockDatabaseService
                .Setup(
                    p => p.ExecuteSqlFromFile<T>(
                        script,
                        It.Is<object>(
                            objectToCheck => ObjectHelpers.ObjectHasPropertyWithExpectedValue(objectToCheck, parameterPropertyName, parameterPropertyValue)
                        )
                    )
                )
                .Returns(returnValue);
        }

        public static void SetupExecuteSqlFromFileGenericWithListParameter<T>(
            this Mock<IDatabaseService> mockDatabaseService,
            string script,
            IDictionary<string, string> literalReplacements,
            string parameterPropertyName,
            IEnumerable<T> parameterPropertyValue,
            IEnumerable<IDictionary<string, object>> returnValue)
        {
            mockDatabaseService
                .Setup(
                    p => p.ExecuteSqlFromFileGeneric(
                        script,
                        literalReplacements,
                        It.Is<object>(
                            objectToCheck => ObjectHelpers.ObjectHasPropertyWithExpectedValue(objectToCheck, parameterPropertyName, parameterPropertyValue)
                        )
                    )
                )
                .Returns(returnValue);
        }

        public static void SetupExecuteSqlFromFile<T>(
            this Mock<IDatabaseService> mockDatabaseService,
            string script,
            IEnumerable<T> returnValue)
        {
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<T>(script))
                .Returns(returnValue);
        }

        public static Mock<IDatabaseService> SetupMockDatabaseService(Instance instance)
        {
            var mockDatabaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            mockDatabaseService.Setup(p => p.ConfigureForInstance(instance));
            return mockDatabaseService;
        }
    }
}