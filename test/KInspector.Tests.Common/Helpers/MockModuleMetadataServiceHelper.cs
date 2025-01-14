using KInspector.Core.Models;
using KInspector.Core.Modules;
using KInspector.Core.Services.Interfaces;

using Moq;

using System.Reflection;

namespace KInspector.Tests.Common.Helpers
{
    public static class MockModuleMetadataServiceHelper
    {
        public static Mock<IModuleMetadataService> GetModuleMetadataService()
        {
            return new Mock<IModuleMetadataService>(MockBehavior.Strict);
        }

        /// <summary>
        /// Sets up <see cref="IModuleMetadataService"/> to return a new <see cref="Labels"/> instead of the real metadata.
        /// This is because the metadata does not influence the data retrieved by the module.
        /// </summary>
        /// <param name="mockModuleMetadataService">Mocked <see cref="IModuleMetadataService"/>.</param>
        /// <param name="module"><see cref="IModule"/> being tested.</param>
        /// <returns><see cref="IModuleMetadataService"/> configured for the <see cref="IModule"/>.</returns>
        public static void SetupModuleMetadataService<T>(Mock<IModuleMetadataService> mockModuleMetadataService, IModule module) where T : new()
        {
            SetupModuleMetadataServiceInternal<T>(module.Codename, mockModuleMetadataService);
        }

        public static Mock<IModuleMetadataService> GetBasicModuleMetadataService<T>(string moduleCodename) where T : new()
        {
            var mockModuleMetadataService = GetModuleMetadataService();

            SetupModuleMetadataServiceInternal<T>(moduleCodename, mockModuleMetadataService);

            return mockModuleMetadataService;
        }

        private static void SetupModuleMetadataServiceInternal<T>(string moduleCodename, Mock<IModuleMetadataService> mockModuleMetadataService) where T : new()
        {
            var fakeMetadata = new ModuleMetadata<T>()
            {
                Terms = new T()
            };

            UpdatePropertiesOfObject(fakeMetadata.Terms);

            mockModuleMetadataService.Setup(p => p.GetModuleMetadata<T>(moduleCodename)).Returns(fakeMetadata);
        }

        private static void UpdatePropertiesOfObject<T>(T objectToUpdate) where T : new()
        {
            var objectProperties = objectToUpdate?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                ?? Enumerable.Empty<PropertyInfo>();

            foreach (var property in objectProperties)
            {
                if (property.PropertyType == typeof(Term))
                {
                    property.SetValue(objectToUpdate, (Term)property.Name);
                }
                else if (property.PropertyType.IsClass)
                {
                    var childObject = Activator.CreateInstance(property.PropertyType);
                    UpdatePropertiesOfObject(childObject);
                    property.SetValue(objectToUpdate, childObject);
                }
            }
        }
    }
}