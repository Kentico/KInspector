using System.Xml;

using KInspector.Core.Constants;
using KInspector.Tests.Common.Helpers;
using KInspector.Reports.SecuritySettingsAnalysis;
using KInspector.Reports.SecuritySettingsAnalysis.Analyzers;
using KInspector.Reports.SecuritySettingsAnalysis.Models;
using KInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KInspector.Reports.SecuritySettingsAnalysis.Models.Results;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public
    class SecuritySettingsAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsSettingsKey> CmsSettingsKeysWithRecommendedValues => new List<CmsSettingsKey>
        {
            new() {
                KeyName = "CMSAutocompleteEnableForLogin",
                KeyValue = "false"
            },
            new() {
                KeyName = "CMSCaptchaControl",
                KeyValue = "3"
            },
            new() {
                KeyName = "CMSChatEnableFloodProtection",
                KeyValue = "true"
            },
            new() {
                KeyName = "CMSFloodProtectionEnabled",
                KeyValue = "true"
            },
            new() {
                KeyName = "CMSForumAttachmentExtensions",
                KeyValue = "jpg;png;zip"
            },
            new() {
                KeyName = "CMSMaximumInvalidLogonAttempts",
                KeyValue = "5"
            },
            new() {
                KeyName = "CMSMediaFileAllowedExtensions",
                KeyValue = "jpg;png;zip"
            },
            new() {
                KeyName = "CMSPasswordExpiration",
                KeyValue = "true"
            },
            new() {
                KeyName = "CMSPasswordExpirationBehaviour",
                KeyValue = "LOCKACCOUNT"
            },
            new() {
                KeyName = "CMSPasswordFormat",
                KeyValue = "PBKDF2"
            },
            new() {
                KeyName = "CMSPolicyMinimalLength",
                KeyValue = "8"
            },
            new() {
                KeyName = "CMSPolicyNumberOfNonAlphaNumChars",
                KeyValue = "2"
            },
            new() {
                KeyName = "CMSRegistrationEmailConfirmation",
                KeyValue = "true"
            },
            new() {
                KeyName = "CMSResetPasswordInterval",
                KeyValue = "6"
            },
            new() {
                KeyName = "CMSRESTServiceEnabled",
                KeyValue = "false"
            },
            new() {
                KeyName = "CMSUploadExtensions",
                KeyValue = "jpg;png;zip"
            },
            new() {
                KeyName = "CMSUsePasswordPolicy",
                KeyValue = "true"
            },
            new() {
                KeyName = "CMSUseSSLForAdministrationInterface",
                KeyValue = "true"
            }
        };

        private IEnumerable<CmsSettingsKey> CmsSettingsKeysWithoutRecommendedValues => new List<CmsSettingsKey>
        {
            new() {
                KeyName = "CMSAutocompleteEnableForLogin",
                KeyDisplayName = "CMSAutocompleteEnableForLogin",
                KeyValue = "true",
                CategoryIDPath = "/1/2/3"
            },
            new() {
                KeyName = "CMSCaptchaControl",
                KeyDisplayName = "CMSCaptchaControl",
                KeyValue = "1",
                CategoryIDPath = "/1/2/3"
            },
            new() {
                KeyName = "CMSForumAttachmentExtensions",
                KeyDisplayName = "CMSForumAttachmentExtensions",
                KeyValue = "jpg;png;zip;exe",
                CategoryIDPath = "/1/2/3"
            },
            new() {
                KeyName = "CMSPolicyMinimalLength",
                KeyDisplayName = "CMSPolicyMinimalLength",
                KeyValue = "7",
                CategoryIDPath = "/1/2/3"
            },
            new() {
                KeyName = "CMSResetPasswordInterval",
                KeyDisplayName = "CMSResetPasswordInterval",
                KeyValue = "13",
                CategoryIDPath = "/1/2/3"
            }
        };

        private IEnumerable<CmsSettingsCategory> CmsSettingsCategoriesWithRecommendedValues => new List<CmsSettingsCategory>();

        private IEnumerable<CmsSettingsCategory> CmsSettingsCategoriesWithoutRecommendedValues => new List<CmsSettingsCategory>
        {
            new() {
                CategoryID = 1,
                CategoryDisplayName = "category1"
            },
            new() {
                CategoryID = 2,
                CategoryDisplayName = "category2"
            },
            new() {
                CategoryID = 3,
                CategoryDisplayName = "category3"
            }
        };

        public string WebConfigWithRecommendedValues => @"Reports\TestData\CMS\WebConfig\webConfigWithRecommendedValues.xml";

        public string WebConfigWithoutRecommendedValues => @"Reports\TestData\CMS\WebConfig\webConfigWithoutRecommendedValues.xml";

        public SecuritySettingsAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(
                _mockDatabaseService.Object,
                _mockInstanceService.Object,
                _mockCmsFileService.Object,
                _mockModuleMetadataService.Object,
                _mockConfigService.Object
                );
        }

        [TestCase(
            Category = "Settings keys and web.config with recommended values",
            TestName = "Settings keys and web.config with recommended values produce a good result"
            )]
        public async Task Should_ReturnGoodResult_When_SettingsKeysAndWebConfigWithRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithRecommendedValues, CmsSettingsCategoriesWithRecommendedValues);
            ArrangeCmsFileService(WebConfigWithRecommendedValues);

            // Act
            var results = await mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries?.Good?.ToString()));
        }

        [TestCase(
            Category = "Settings keys without recommended values and web.config with recommended values",
            TestName = "Settings keys without recommended values and web.config with recommended values produce a warning result"
            )]
        public async Task Should_ReturnWarningResult_When_SettingsKeysWithoutRecommendedValuesAndWebConfigWithRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithoutRecommendedValues, CmsSettingsCategoriesWithoutRecommendedValues);
            ArrangeCmsFileService(WebConfigWithRecommendedValues);

            // Act
            var results = await mockReport.GetResults();
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.AdminSecuritySettings) ?? false);

            // Assert
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count(), Is.EqualTo(5));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries?.Warning?.ToString()));
        }

        [TestCase(
            Category = "Settings keys with recommended values and web.config without recommended values",
            TestName = "Settings keys with recommended values and web.config without recommended values produce a warning result"
            )]
        public async Task Should_ReturnWarningResult_When_SettingsKeysWithRecommendedValuesAndWebConfigWithoutRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithRecommendedValues, CmsSettingsCategoriesWithRecommendedValues);
            ArrangeCmsFileService(WebConfigWithoutRecommendedValues);

            // Act
            var results = await mockReport.GetResults();
            var webConfigTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.WebConfigSecuritySettings) ?? false);

            // Assert
            Assert.That(webConfigTable, Is.Not.Null);
            Assert.That(webConfigTable?.Rows.Count(), Is.EqualTo(8));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries?.Warning?.ToString()));
        }

        [TestCase(
            Category = "Settings keys without recommended values and web.config without recommended values",
            TestName = "Settings keys without recommended values and web.config without recommended values produce a warning result"
            )]
        public async Task Should_ReturnWarningResult_When_SettingsKeysWithoutRecommendedValuesAndWebConfigWithoutRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithoutRecommendedValues, CmsSettingsCategoriesWithoutRecommendedValues);
            ArrangeCmsFileService(WebConfigWithoutRecommendedValues);

            // Act
            var results = await mockReport.GetResults();
            var webConfigTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.WebConfigSecuritySettings) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(mockReport.Metadata.Terms.TableTitles?.AdminSecuritySettings) ?? false);

            // Assert
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(webConfigTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count(), Is.EqualTo(5));
            Assert.That(webConfigTable?.Rows.Count(), Is.EqualTo(8));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries?.Warning?.ToString()));
        }

        private void ArrangeDatabaseService(
            IEnumerable<CmsSettingsKey> cmsSettingsKeys,
            IEnumerable<CmsSettingsCategory> cmsSettingsCategories
            )
        {
            var cmsSettingsKeysNames = new SettingsKeyAnalyzers(null)
                .Analyzers
                .Select(analyzer => analyzer.Parameters[0].Name);

            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(
                Scripts.GetSecurityCmsSettings,
                nameof(cmsSettingsKeysNames),
                cmsSettingsKeysNames,
                cmsSettingsKeys);

            var cmsSettingsCategoryIdsOnPaths = cmsSettingsCategories
                .Select(category => category.CategoryID.ToString());

            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(
                Scripts.GetCmsSettingsCategories,
                nameof(cmsSettingsCategoryIdsOnPaths),
                cmsSettingsCategoryIdsOnPaths,
                cmsSettingsCategories
                );
        }

        private void ArrangeCmsFileService(string webConfigPath)
        {
            var webConfig = new XmlDocument();
            webConfig.Load(webConfigPath);

            _mockCmsFileService
                .Setup(p => p.GetXmlDocument(_mockInstance.AdministrationPath, DefaultKenticoPaths.WebConfigFile))
                .Returns(webConfig);

            _mockCmsFileService
                .Setup(p => p.GetResourceStringsFromResx(_mockInstance.AdministrationPath, DefaultKenticoPaths.PrimaryResxFile))
                .Returns(new Dictionary<string, string>());
        }
    }
}