using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;
using Moq;
using System;
using System.Reflection;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockReportMetadataServiceHelper
    {
        public static Mock<IModuleMetadataService> GetReportMetadataService()
        {
            return new Mock<IModuleMetadataService>(MockBehavior.Strict);
        }

        /// <summary>
        /// Sets up <see cref="IModuleMetadataService"/> to return a new <see cref="Labels"/> instead of the real metadata.
        /// This is because the metadata does not influence the data retrieved by the report.
        /// </summary>
        /// <param name="mockReportMetadataService">Mocked <see cref="IModuleMetadataService"/>.</param>
        /// <param name="report"><see cref="IReport"/> being tested.</param>
        /// <returns><see cref="IModuleMetadataService"/> configured for the <see cref="IReport"/>.</returns>
        public static void SetupReportMetadataService<T>(Mock<IModuleMetadataService> mockReportMetadataService, IReport report) where T : new()
        {
            SetupReportMetadataServiceInternal<T>(report.Codename, mockReportMetadataService);
        }

        public static Mock<IModuleMetadataService> GetBasicReportMetadataService<T>(string reportCodename) where T : new()
        {
            var mockReportMetadataService = GetReportMetadataService();

            SetupReportMetadataServiceInternal<T>(reportCodename, mockReportMetadataService);

            return mockReportMetadataService;
        }

        private static void SetupReportMetadataServiceInternal<T>(string reportCodename, Mock<IModuleMetadataService> mockReportMetadataService) where T : new()
        {
            var fakeMetadata = new ModuleMetadata<T>()
            {
                Terms = new T()
            };

            UpdatePropertiesOfObject(fakeMetadata.Terms);

            mockReportMetadataService.Setup(p => p.GetModuleMetadata<T>(reportCodename)).Returns(fakeMetadata);
        }

        private static void UpdatePropertiesOfObject<T>(T objectToUpdate) where T : new()
        {
            var objectProperties = objectToUpdate.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

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