using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

using Moq;

namespace KInspector.Tests.Common.Helpers
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
                .Returns(Task.FromResult(returnValue));
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
                .Returns(Task.FromResult(returnValue));
        }

        public static void SetupExecuteSqlFromFile<T>(
            this Mock<IDatabaseService> mockDatabaseService,
            string script,
            IEnumerable<T> returnValue)
        {
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<T>(script))
                .Returns(Task.FromResult(returnValue));
        }

        public static Mock<IDatabaseService> SetupMockDatabaseService(Instance instance)
        {
            var mockDatabaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            mockDatabaseService.Setup(p => p.Configure(instance.DatabaseSettings));

            return mockDatabaseService;
        }
    }
}