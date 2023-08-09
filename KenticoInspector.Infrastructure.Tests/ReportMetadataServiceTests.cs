using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Core.Tokens;
using KenticoInspector.Modules.Tests.Helpers;

using NUnit.Framework;

namespace KenticoInspector.Infrastructure.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class ReportMetadataServiceTests
    {
        private readonly IModuleMetadataService moduleMedatadataService;

        public class TestTerms
        {
            public Term SingleTerm { get; set; }
        }

        public ReportMetadataServiceTests(int majorVersion)
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(Term).Assembly);

            var mockInstance = MockInstances.Get(majorVersion);
            var mockInstanceDetails = MockInstanceDetails.Get(majorVersion, mockInstance);
            var mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(mockInstance, mockInstanceDetails);

            moduleMedatadataService = new ModuleMetadataService(mockInstanceService.Object);
        }

        [TestCaseSource(typeof(YamlTestCases), nameof(YamlTestCases.YamlMatchesModel))]
        public void Should_Resolve_When_YamlMatchesModel(
            string cultureName,
            string yamlPath,
            ModuleMetadata<TestTerms> resolvedMetadata)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            // Act
            var metadata = moduleMedatadataService.GetModuleMetadata<TestTerms>(yamlPath);

            // Assert
            Assert.That(metadata.Details.Name, Is.EqualTo(resolvedMetadata.Details.Name));
            Assert.That(metadata.Details.ShortDescription, Is.EqualTo(resolvedMetadata.Details.ShortDescription));
            Assert.That(metadata.Details.LongDescription, Is.EqualTo(resolvedMetadata.Details.LongDescription));
            Assert.That(metadata.Terms.SingleTerm.ToString(), Is.EqualTo(resolvedMetadata.Terms.SingleTerm.ToString()));
        }

        [TestCase("en-US", "TestData\\YamlDoesNotMatch", TestName = "Metadata throws exception when YAML does not match the model.")]
        public void Should_Throw_When_YamlDoesNotMatchModel(
            string cultureName,
            string yamlPath)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            // Act
            ModuleMetadata<TestTerms> getReportMetadata(string path) => moduleMedatadataService
                    .GetModuleMetadata<TestTerms>(path);

            // Assert
            Assert.That(() => getReportMetadata(yamlPath), Throws.Exception);
        }

        public static class YamlTestCases
        {
            public static IEnumerable<TestCaseData> YamlMatchesModel
            {
                get
                {
                    yield return GetTestCaseData("en-US", "TestData\\YamlMatches", new ModuleMetadata<TestTerms>()
                    {
                        Details = new ModuleDetails()
                        {
                            Name = "Details name",
                            ShortDescription = "Details shortDescription",
                            LongDescription = "Details longDescription\n"
                        },
                        Terms = new TestTerms()
                        {
                            SingleTerm = "Term value"
                        }
                    });
                    yield return GetTestCaseData("en-GB", "TestData\\YamlMatches", new ModuleMetadata<TestTerms>()
                    {
                        Details = new ModuleDetails()
                        {
                            Name = "British details name",
                            ShortDescription = "Details shortDescription",
                            LongDescription = "Details longDescription\n"
                        },
                        Terms = new TestTerms()
                        {
                            SingleTerm = "British term value"
                        }
                    });
                }
            }

            private static TestCaseData GetTestCaseData(
                string cultureCode,
                string yamlPath,
                ModuleMetadata<TestTerms> resolvedMetadata)
            {
                return new TestCaseData(cultureCode, yamlPath, resolvedMetadata);
                //TODO: add .SetName($"Metadata in culture \"{cultureCode}\" resolves."); once NUnit fixes https://github.com/nunit/nunit3-vs-adapter/issues/607
            }
        }
    }
}