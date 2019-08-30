using System.Collections.Generic;
using System.Linq;
using System.Xml;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Results;
using KenticoInspector.Reports.Tests.Helpers;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public
    class SecuritySettingsAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsSettingsKey> CmsSettingsKeysWithRecommendedValues => new List<CmsSettingsKey>
        {
            new CmsSettingsKey
            {
                KeyName = "CMSAutocompleteEnableForLogin",
                KeyValue = "false"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSCaptchaControl",
                KeyValue = "3"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSChatEnableFloodProtection",
                KeyValue = "true"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSFloodProtectionEnabled",
                KeyValue = "true"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSForumAttachmentExtensions",
                KeyValue = "jpg;png;zip"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSMaximumInvalidLogonAttempts",
                KeyValue = "5"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSMediaFileAllowedExtensions",
                KeyValue = "jpg;png;zip"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSPasswordExpiration",
                KeyValue = "true"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSPasswordExpirationBehaviour",
                KeyValue = "LOCKACCOUNT"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSPasswordFormat",
                KeyValue = "PBKDF2"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSPolicyMinimalLength",
                KeyValue = "8"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSPolicyNumberOfNonAlphaNumChars",
                KeyValue = "2"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSRegistrationEmailConfirmation",
                KeyValue = "true"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSResetPasswordInterval",
                KeyValue = "6"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSRESTServiceEnabled",
                KeyValue = "false"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSUploadExtensions",
                KeyValue = "jpg;png;zip"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSUsePasswordPolicy",
                KeyValue = "true"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSUseSSLForAdministrationInterface",
                KeyValue = "true"
            }
        };

        private IEnumerable<CmsSettingsKey> CmsSettingsKeysWithoutRecommendedValues => new List<CmsSettingsKey>
        {
            new CmsSettingsKey
            {
                KeyName = "CMSAutocompleteEnableForLogin",
                KeyDisplayName = "CMSAutocompleteEnableForLogin",
                KeyValue = "true",
                CategoryIDPath = "/1/2/3"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSCaptchaControl",
                KeyDisplayName = "CMSCaptchaControl",
                KeyValue = "1",
                CategoryIDPath = "/1/2/3"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSForumAttachmentExtensions",
                KeyDisplayName = "CMSForumAttachmentExtensions",
                KeyValue = "jpg;png;zip;exe",
                CategoryIDPath = "/1/2/3"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSPolicyMinimalLength",
                KeyDisplayName = "CMSPolicyMinimalLength",
                KeyValue = "7",
                CategoryIDPath = "/1/2/3"
            },
            new CmsSettingsKey
            {
                KeyName = "CMSResetPasswordInterval",
                KeyDisplayName = "CMSResetPasswordInterval",
                KeyValue = "13",
                CategoryIDPath = "/1/2/3"
            }
        };

        private IEnumerable<CmsSettingsCategory> CmsSettingsCategoriesWithRecommendedValues => new List<CmsSettingsCategory>();

        private IEnumerable<CmsSettingsCategory> CmsSettingsCategoriesWithoutRecommendedValues => new List<CmsSettingsCategory>
        {
            new CmsSettingsCategory
            {
                CategoryID = 1,
                CategoryDisplayName = "category1"
            },
            new CmsSettingsCategory
            {
                CategoryID = 2,
                CategoryDisplayName = "category2"
            },
            new CmsSettingsCategory
            {
                CategoryID = 3,
                CategoryDisplayName = "category3"
            }
        };

        public string WebConfigWithRecommendedValues => @"TestData\CMS\WebConfig\webConfigWithRecommendedValues.xml";

        public string WebConfigWithoutRecommendedValues => @"TestData\CMS\WebConfig\webConfigWithoutRecommendedValues.xml";

        public SecuritySettingsAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(
                _mockDatabaseService.Object,
                _mockInstanceService.Object,
                _mockCmsFileService.Object,
                _mockReportMetadataService.Object
                );
        }

        [TestCase(
            Category = "Settings keys and web.config with recommended values",
            TestName = "Settings keys and web.config with recommended values produce a good result"
            )]
        public void Should_ReturnGoodResult_When_SettingsKeysAndWebConfigWithRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithRecommendedValues, CmsSettingsCategoriesWithRecommendedValues);
            ArrangeCmsFileService(WebConfigWithRecommendedValues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries.Good.ToString()));
        }

        [TestCase(
            Category = "Settings keys without recommended values and web.config with recommended values",
            TestName = "Settings keys without recommended values and web.config with recommended values produce a warning result"
            )]
        public void Should_ReturnWarningResult_When_SettingsKeysWithoutRecommendedValuesAndWebConfigWithRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithoutRecommendedValues, CmsSettingsCategoriesWithoutRecommendedValues);
            ArrangeCmsFileService(WebConfigWithRecommendedValues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries.Warning.ToString()));

            Assert.That(GetResult<TableResult<CmsSettingsKeyResult>>(results).Rows.Count(), Is.EqualTo(5));
        }

        [TestCase(
            Category = "Settings keys with recommended values and web.config without recommended values",
            TestName = "Settings keys with recommended values and web.config without recommended values produce a warning result"
            )]
        public void Should_ReturnWarningResult_When_SettingsKeysWithRecommendedValuesAndWebConfigWithoutRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithRecommendedValues, CmsSettingsCategoriesWithRecommendedValues);
            ArrangeCmsFileService(WebConfigWithoutRecommendedValues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries.Warning.ToString()));

            Assert.That(GetResult<TableResult<WebConfigSettingResult>>(results).Rows.Count(), Is.EqualTo(8));
        }

        [TestCase(
            Category = "Settings keys without recommended values and web.config without recommended values",
            TestName = "Settings keys without recommended values and web.config without recommended values produce a warning result"
            )]
        public void Should_ReturnWarningResult_When_SettingsKeysWithoutRecommendedValuesAndWebConfigWithoutRecommendedValues()
        {
            // Arrange
            ArrangeDatabaseService(CmsSettingsKeysWithoutRecommendedValues, CmsSettingsCategoriesWithoutRecommendedValues);
            ArrangeCmsFileService(WebConfigWithoutRecommendedValues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries.Warning.ToString()));

            Assert.That(GetResult<TableResult<CmsSettingsKeyResult>>(results).Rows.Count(), Is.EqualTo(5));
            Assert.That(GetResult<TableResult<WebConfigSettingResult>>(results).Rows.Count(), Is.EqualTo(8));
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
                .Setup(p => p.GetXmlDocument(_mockInstance.Path, DefaultKenticoPaths.WebConfigFile))
                .Returns(webConfig);

            _mockCmsFileService
                .Setup(p => p.GetResourceStringsFromResx(_mockInstance.Path, DefaultKenticoPaths.PrimaryResxFile))
                .Returns(new Dictionary<string, string>());
        }

        private static TResult GetResult<TResult>(ReportResults results)
        {
            IDictionary<string, object> dictionaryData = results.Data;

            return dictionaryData
                .Values
                .OfType<TResult>()
                .First();
        }
    }
}