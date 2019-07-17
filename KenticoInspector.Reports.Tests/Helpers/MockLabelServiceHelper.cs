using System.Reflection;
using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using Moq;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockLabelServiceHelper
    {
        public static Mock<ILabelService> GetlabelService()
        {
            return new Mock<ILabelService>(MockBehavior.Strict);
        }

        /// <summary>
        /// Sets up <see cref="ILabelService"/> to return a new <see cref="Labels"/> instead of the real labels.
        /// This is because the labels are labels and do not influence the data retrieved by the report.
        /// </summary>
        /// <param name="mockLabelService">Mocked <see cref="ILabelService"/>.</param>
        /// <param name="report"><see cref="IReport"/> being tested.</param>
        /// <returns><see cref="ILabelService"/> configured for the <see cref="IReport"/>.</returns>
        public static Mock<ILabelService> SetuplabelService<TLabels>(Mock<ILabelService> mockLabelService, IReport report) where TLabels : new()
        {
            var fakeMetadata = new Metadata<TLabels>()
            {
                Labels = new TLabels()
            };

            var properties = fakeMetadata.Labels.GetType()
                                        .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                property.SetValue(fakeMetadata.Labels, (Label)property.Name);
            }

            mockLabelService.Setup(p => p.GetMetadata<TLabels>(report.Codename)).Returns(fakeMetadata);

            return mockLabelService;
        }
    }
}