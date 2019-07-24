using System;
using System.Reflection;
using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using Moq;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockReportMetadataServiceHelper
    {
        public static Mock<IReportMetadataService> GetReportMetadataService()
        {
            return new Mock<IReportMetadataService>(MockBehavior.Strict);
        }

        /// <summary>
        /// Sets up <see cref="IReportMetadataService"/> to return a new <see cref="Labels"/> instead of the real metadata.
        /// This is because the metadata does not influence the data retrieved by the report.
        /// </summary>
        /// <param name="mockReportMetadataService">Mocked <see cref="IReportMetadataService"/>.</param>
        /// <param name="report"><see cref="IReport"/> being tested.</param>
        /// <returns><see cref="IReportMetadataService"/> configured for the <see cref="IReport"/>.</returns>
        public static Mock<IReportMetadataService> SetupReportMetadataService<T>(Mock<IReportMetadataService> mockReportMetadataService, IReport report) where T : new()
        {
            var fakeMetadata = new ReportMetadata<T>()
            {
                Terms = new T()
            };

            UpdatePropertiesOfObject(fakeMetadata.Terms);

            mockReportMetadataService.Setup(p => p.GetReportMetadata<T>(report.Codename)).Returns(fakeMetadata);

            return mockReportMetadataService;
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
                else if (property.PropertyType.IsClass) {
                    var childObject = Activator.CreateInstance(property.PropertyType);
                    UpdatePropertiesOfObject(childObject);
                    property.SetValue(objectToUpdate, childObject);
                }
            }
        }
    }
}