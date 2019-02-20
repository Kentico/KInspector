using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Resources;

using Kentico.KInspector.Core;
using Kentico.KInspector.Modules.Helpers.StrongTypedInfos;

namespace Kentico.KInspector.Modules
{
    public class RecommendedSettingsModule : IModule
    {
        private const string KEYSTABLE_TYPENAME = "KI_SettingsKeysNamesList";
        private const string PATH_SEPARATOR = " > ";

        private static ResXResourceSet _CMSResxSet;

        private static readonly string _keysTableParameterInitializationSQL = $@"
IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='{KEYSTABLE_TYPENAME}')
    CREATE TYPE dbo.{KEYSTABLE_TYPENAME} AS TABLE(
        {nameof(KeyInfo.KeyName)} VARCHAR(MAX),
        {nameof(KeyInfo.RecommendedValue)} VARCHAR(MAX),
        {nameof(KeyInfo.Notes)} VARCHAR(MAX)
    )
        ";

        private static readonly string _keysTableParameterDisposalSQL = $"DROP TYPE [dbo].[{KEYSTABLE_TYPENAME}] ";

        private static readonly IDictionary<string, KeyData> _keyDataDictionary = new Dictionary<string, KeyData>
        {
            // Settings > Content
            { "CMSPageNotFoundUrl", new KeyData(Recommended.IS_NOT_EMPTY, Recommended.NOT_EMPTY, "We recommend that you configure the setting 'Page not found URL'." ) },
            { "CMSLogPageNotFoundException", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend that you enable logging of page not found exceptions and configure reporting for this if you are not tracking 404s in some other way." ) },
            { "CMSPageTitleFormat", new KeyData(Recommended.IS_NOT_EMPTY, Recommended.NOT_EMPTY, "We recommend that the macro 'Page title prefix' actually come after the macro 'Page title or else name' in the setting 'Page title format' as this is an SEO best practice." ) },
            { "CMSMediaFileAllowedExtensions", new KeyData(null, Recommended.CONFIGURED, "Check for possible dangerous extensions (.exe, .src, ...)") },

            // Settings > URLs and SEO
            { "CMSFriendlyURLExtension", new KeyData(Recommended.IS_EMPTY, Recommended.EMPTY, "We recommend configuring this to be blank or start with a semi-colon to make URLs extension-less by default." ) },
            { "CMSFilesFriendlyURLExtension", new KeyData(Recommended.IS_EMPTY, Recommended.EMPTY, "We recommend configuring this to be blank or start with a semi-colon to make URLs extension-less by default." ) },
            { "CMSUsePermanentURLs", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend that the Use permanent URLs check box be unchecked so that URLs are readable and SEO friendly." ) },
            { "CMSKeepChangedDocumentAccesible", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend that the Remember original URLs when moving pages check box be checked in production to prevent accidental 404s. This option should usually be disabled during development." ) },
            { "CMSGoogleSitemapURL", new KeyData(Recommended.IS_NOT_EMPTY, Recommended.CONFIGURED, "We recommend that you customize the google sitemap to include the site’s custom page types. We recommend configuring the runAllManagedModulesForAllRequests attribute in order for the sitemap to be served for the .xml file extension." ) },
            { "CMSRobotsPath", new KeyData(null, Recommended.CONFIGURED, "We recommend that a robots.txt file be set up. This does not have to be done via the CMS functionality, but it should load." ) },
            { "CMSUsePermanentRedirect", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend enabling this if the site is well established." ) },
            { "CMSMoveViewStateToEnd", new KeyData(Recommended.IS_TRUE,KeySeverity.Warning, Recommended.TRUE, "We recommend enabling this for better SEO." ) },
            { "CMSDefaultDeletedNodePath", new KeyData(null, "Not 404 page", "We recommend configuring this to a page that explains that the page is no longer available rather than relying on the 404 page." ) },
            { "CMSUseURLsWithTrailingSlash", new KeyData(s => !s.Equals("DONTCARE"), Recommended.CONFIGURED, "We recommend configuring the settings in the SEO – URLs section to force consistent URLs." ) },
            { "CMSProcessDomainPrefix", new KeyData(null, Recommended.CONFIGURED, "We recommend requiring that the domain be configured to use the www prefix or not. This is best handled with the IIS rewrite module." ) },
            { "CMSUseLangPrefixForUrls", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend enabling this if your site is multilingual and you don’t have domains for each language (e.g. en-us.domain.com)." ) },

            // Settings > Security & Membership
            { "CMSRegistrationEmailConfirmation", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSUseSSLForAdministrationInterface", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSPasswordFormat", new KeyData(s => s.Equals("PBKDF2"), KeySeverity.Critical, "PBKDF2", "For versions 10 and higher, use PBKDF2.") },
            { "CMSPasswordExpiration", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSPasswordExpirationBehaviour", new KeyData(s => s.Equals("LOCKACCOUNT"), "Lock account") },
            { "CMSUsePasswordPolicy", new KeyData(Recommended.IS_TRUE, KeySeverity.Warning, Recommended.TRUE) },
            { "CMSPolicyMinimalLength", new KeyData(s => int.Parse(s) > 7, "8+") },
            { "CMSPolicyNumberOfNonAlphaNumChars", new KeyData(s => int.Parse(s) > 1, "2+") },
            { "CMSAutocompleteEnableForLogin", new KeyData(Recommended.IS_FALSE, KeySeverity.Warning, Recommended.FALSE) },
            { "CMSFloodProtectionEnabled", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSCaptchaControl", new KeyData(s => s.Equals("3"), "reCAPTCHA", "We recommend setting this to reCAPTCHA and configuring the reCAPTCHA settings." ) },

            // Settings > System
            { "CMSLogSize", new KeyData(s => int.Parse(s) > 5000,KeySeverity.Warning, "5,000 - 10,000") },
            { "CMSNoreplyEmailAddress", new KeyData(null, Recommended.CONFIGURED, "We recommend configuring this setting to an email on your domain.") },
            { "CMSSendErrorNotificationTo", new KeyData(null, Recommended.CONFIGURED, "We recommend configuring this setting to an administrator’s email address or group.") },
            { "CMSSendEmailNotificationsFrom", new KeyData(null, Recommended.CONFIGURED, "We recommend configuring this setting to an email on your domain.") },
            { "CMSSchedulerTasksEnabled", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSSchedulerUseExternalService", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend enabling this setting in conjunction with configuring the external service on all IIS servers. (EMS only).") },
            { "CMSServerTimeZone", new KeyData(null, Recommended.CONFIGURED, "We recommend setting this setting to the same value as the host OS. This is also required for the Site time zone setting to work.") },
            { "CMSSiteTimeZone", new KeyData(null, Recommended.CONFIGURED) },
            { "CMSModuleUsageTrackingEnabled", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend considering enabling this setting to help us better focus our development efforts.") },
            { "CMSAllowGZip", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSCachePageInfo", new KeyData(s => int.Parse(s) > 10, "Higher than 10 minutes") },
            { "CMSCacheMinutes", new KeyData(s => int.Parse(s) > 10, "Higher than 10 minutes") },
            { "CMSProgressiveCaching", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSCacheImages", new KeyData(s => int.Parse(s) > 10, "Higher than 10 minutes") },
            { "CMSMaxCacheFileSize", new KeyData(null, Recommended.CONFIGURED, "We recommend that you consider increasing this setting in conjunction with enabling the setting 'Redirect files to disk' if you serve a lot of files from attachments or the content tree.") },
            { "CMSRedirectFilesToDisk", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend that you consider enabling this setting if you serve a lot of files from attachments or the content tree.") },
            { "CMSClientCacheMinutes", new KeyData(s => int.Parse(s) > 10080, "10,080 minutes to 525,600 minutes", "We recommend setting this to a minimum of one week (10,080 minutes) and up to a year (525,600 minutes) per Google’s recommendation (https://developers.google.com/speed/docs/insights/LeverageBrowserCaching?hl=en). For assets that may change in the cache lifetime, we recommend implementing URL versioning (e.g. /asset.ext?v=20151231-1).") },
            { "CMSRevalidateClientCache", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "Setting this can prevent additional, unnecessary HTTP requests.") },
            { "CMSEnableOutputCache", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSEnablePartialCache", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSResourceCompressionEnabled", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSScriptMinificationEnabled", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSStylesheetMinificationEnabled", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSResolveMacrosInCSS", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling this setting unless you are utilizing macros in CSS.") },
            { "CMSAllowComponentsCSS", new KeyData(Recommended.IS_FALSE, KeySeverity.Warning, Recommended.FALSE, "We recommend disabling this setting and manually migrating the necessary styles into a global stylesheet.") },
            { "CMSEmailQueueEnabled", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSArchiveEmails", new KeyData(s => int.Parse(s) > 1, Recommended.CONFIGURED, "We recommend setting this to at least 1 day unless you have high email volume.") },
            { "CMSFilesLocationType", new KeyData(s => int.Parse(s) > 0, "File system or database and file system") },
            { "CMSUploadExtensions", new KeyData(null, Recommended.CONFIGURED, "Check for possible dangerous extensions (.exe, .src, ...)") },
            { "CMSCheckPublishedFiles", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling this setting unless business logic requires it.") },
            { "CMSCheckFilesPermissions", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling this setting unless business logic requires it.") },
            { "CMSExcludedXHTMLFilterURLs", new KeyData(s => s.Equals("/"), "/", "We recommend excluding all pages on the website from the output filter when all code produced on the site is valid by setting this to '/'. You can enable the output filter on specific web parts, such as the editable text web part, when you can’t guarantee valid output.") },
            { "CMSExcludeDocumentsFromSearch", new KeyData(null, Recommended.CONFIGURED, "We recommend excluding appropriate page types and pages/paths from SQL search as appropriate.") },
            { "CMSDisableDebug", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling this setting in a production environment.") },

            // Settings > Online marketing
            { "CMSEnableOnlineMarketing", new KeyData(Recommended.IS_FALSE, KeySeverity.Warning, Recommended.CONFIGURED, "We recommend enabling this setting only if you are using online marketing features.") },
            { "CMSABTestingEnabled", new KeyData(Recommended.IS_FALSE, KeySeverity.Warning, Recommended.CONFIGURED, "We recommend enabling this setting only if you are using A/B testing.") },
            { "CMSMVTEnabled", new KeyData(Recommended.IS_FALSE, KeySeverity.Warning, Recommended.CONFIGURED, "We recommend enabling this setting only if you are using MV testing.") },
            { "CMSContentPersonalizationEnabled", new KeyData(Recommended.IS_FALSE, KeySeverity.Warning, Recommended.CONFIGURED, "We recommend enabling this setting only if you are using content personalization.") },
            { "CMSCMActivitiesEnabled", new KeyData(Recommended.IS_FALSE, KeySeverity.Warning, Recommended.CONFIGURED, "We recommend enabling this setting only if you are logging activities.") },
            { "CMSCMPageVisits", new KeyData(null, null) },
            { "CMSCMLandingPage", new KeyData(null, null) },
            { "CMSCMUserRegistration", new KeyData(null, null) },
            { "CMSCMUserLogin", new KeyData(null, null) },
            { "CMSCMAddingProductToSC", new KeyData(null, null) },
            { "CMSCMRemovingProductFromSC", new KeyData(null, null) },
            { "CMSCMAddingProductToWL", new KeyData(null, null) },
            { "CMSCMPurchase", new KeyData(null, null) },
            { "CMSCMPurchasedProduct", new KeyData(null, null) },
            { "CMSCMNewsletterSubscribe", new KeyData(null, null) },
            { "CMSCMNewsletterUnsubscribe", new KeyData(null, null) },
            { "CMSCMNewsletterUnsubscribedFromAll", new KeyData(null, null) },
            { "CMSCMEmailOpening", new KeyData(null, null) },
            { "CMSCMClickthroughTracking", new KeyData(null, null) },
            { "CMSCMSearch", new KeyData(null, null) },
            { "CMSCMExternalSearch", new KeyData(null, null) },
            { "CMSCMBlogPostSubscription", new KeyData(null, null) },
            { "CMSCMBlogPostComments", new KeyData(null, null) },
            { "CMSCMForumPostSubscription", new KeyData(null, null) },
            { "CMSCMForumPosts", new KeyData(null, null) },
            { "CMSCMMessageBoardSubscription", new KeyData(null, null) },
            { "CMSCMMessageBoardPosts", new KeyData(null, null) },
            { "CMSCMBizFormSubmission", new KeyData(null, null) },
            { "CMSCMContentRating", new KeyData(null, null) },
            { "CMSCMPollVoting", new KeyData(null, null) },
            { "CMSCMCustomTableForm", new KeyData(null, null) },
            { "CMSCMEventBooking", new KeyData(null, null) },
            { "CMSCMCustomActivities", new KeyData(null, null) },
            { "CMSDeleteInactiveContactsMethod", new KeyData(Recommended.IS_NOT_EMPTY, Recommended.NOT_EMPTY, "We recommend configuring the inactive contact deletion settings. For more information, run the module 'Inactive contact deletion settings'.") },
            { "CMSNewsletterUnsubscriptionURL", new KeyData(null, Recommended.CONFIGURED, "We recommend creating a page and configuring this setting to point to that page only if you are using the email marketing features.") },
            { "CMSOptInApprovalURL", new KeyData(null, Recommended.CONFIGURED, "We recommend creating a page and configuring this setting to point to that page only if you are using the email marketing features.") },
            { "CMSAnalyticsEnabled", new KeyData(null, Recommended.CONFIGURED, "We recommend enabling this setting only if you are using web analytics.") },
            { "CMSWebAnalyticsUseJavascriptLogging", new KeyData(Recommended.IS_TRUE, Recommended.TRUE) },
            { "CMSAnalyticsExcludeSearchEngines", new KeyData(Recommended.IS_TRUE, Recommended.TRUE, "We recommend enabling this setting to prevent search engine bot traffic from logging analytics data.") },
            { "CMSAnalyticsExcludedIPs", new KeyData(null, Recommended.CONFIGURED, "We recommend adding your company’s IP addresses to this setting to prevent internal traffic from logging analytics data.") },
            { "CMSAnalyticsTrackFileDownloads", new KeyData(null, null) },
            { "CMSAnalyticsTrackInvalidPages", new KeyData(null, null) },
            { "CMSAnalyticsTrackPageViews", new KeyData(null, null) },
            { "CMSAnalyticsTrackAggregatedViews", new KeyData(null, null) },
            { "CMSTrackLandingPages", new KeyData(null, null) },
            { "CMSTrackExitPages", new KeyData(null, null) },
            { "CMSTrackAverageTimeOnPage", new KeyData(null, null) },
            { "CMSAnalyticsTrackBrowserTypes", new KeyData(null, null) },
            { "CMSAnalyticsTrackVisits", new KeyData(null, null) },
            { "CMSAnalyticsTrackCountries", new KeyData(null, null) },
            { "CMSAnalyticsTrackRegisteredUsers", new KeyData(null, null) },
            { "CMSTrackSearchCrawlers", new KeyData(null, null) },
            { "CMSTrackMobileDevices", new KeyData(null, null) },
            { "CMSTrackSearchEngines", new KeyData(null, null) },
            { "CMSTrackSearchKeywords", new KeyData(null, null) },
            { "CMSTrackOnSiteKeywords", new KeyData(null, null) },
            { "CMSTrackReferringSitesLocal", new KeyData(null, null) },
            { "CMSTrackReferringSitesDirect", new KeyData(null, null) },
            { "CMSTrackReferringSitesReferring", new KeyData(null, null) },
            { "CMSAnalyticsTrackReferrals", new KeyData(null, null) },

            // Settings > Versioning & Synchronization
            { "CMSStagingServiceEnabled", new KeyData(null, null, "We recommend that you do not stage content from production to staging. No changes should be made in production as they come from staging.") },
            { "CMSStagingLogChanges", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling logging of all changes in the production environment.") },
            { "CMSStagingLogDataChanges", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling logging of all changes in the production environment.") },
            { "CMSStagingLogObjectChanges", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling logging of all changes in the production environment.") },
            { "CMSStagingLogStagingChanges", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling logging of all changes in the production environment.") },
            { "CMSExportLogObjectChanges", new KeyData(Recommended.IS_FALSE, Recommended.FALSE, "We recommend disabling logging of all changes in the production environment.") },

            // Settings > Versioning & Synchronization
            { "CMSRESTServiceEnabled", new KeyData(Recommended.IS_FALSE, Recommended.CONFIGURED, "We recommend enabling this setting only if you are using the feature.") },
        };

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Recommended settings for given site",
                SupportedVersions = new[] {
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0"),
                    new Version("12.0")
                },
                Comment = @"Lists settings and recommended values. If there are no recommendations, it means that the setting on the site level, or on the global level if there is no site level override, fits our recommendations.",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            // Prepare table valued type
            instanceInfo.DBService.ExecuteAndGetDataSet(_keysTableParameterInitializationSQL);

            var results = instanceInfo.DBService.ExecuteAndGetDataSetFromFile(
                "RecommendedSettingsModule.sql",
                new SqlParameter("domain", instanceInfo.Uri.Host),
                GetDataTableParameter("keyNames",
                    KEYSTABLE_TYPENAME,
                    new[] {
                        nameof(KeyInfo.KeyName),
                        nameof(KeyInfo.RecommendedValue),
                        nameof(KeyInfo.Notes)
                    },
                    _keyDataDictionary
                )
            );

            // Dispose table valued type
            instanceInfo.DBService.ExecuteAndGetDataSet(_keysTableParameterDisposalSQL);

            DataSet finalDataSet;
            int[] keySeverities = { 0, 0, 0 };
            Status resultStatus = Status.Good;

            using (_CMSResxSet = GetCMSResXResourceSet(instanceInfo.Directory))
            {
                finalDataSet = BuildDataSet(results.Tables[1], results.Tables[0], keySeverities);
            }

            if (keySeverities[0] > 0)
                resultStatus = Status.Info;

            if (keySeverities[1] > 0)
                resultStatus = Status.Warning;

            if (keySeverities[2] > 0)
                resultStatus = Status.Error;

            return new ModuleResults
            {
                Result = finalDataSet,
                Status = resultStatus
            };
        }

        private static ResXResourceSet GetCMSResXResourceSet(DirectoryInfo directory)
        {
            var resXPath = directory.ToString() + "\\CMS\\CMSResources\\CMS.resx";

            if (File.Exists(resXPath))
            {
                return new ResXResourceSet(resXPath);
            }

            return null;
        }

        private static SqlParameter GetDataTableParameter(string parameterName, string tableName, string[] columnNames, IDictionary<string, KeyData> rows)
        {
            var table = new DataTable(tableName);

            foreach (var columnName in columnNames)
            {
                table.Columns.Add(columnName);
            }

            foreach (var row in rows)
            {
                // Add an array of the key and fields
                table.Rows.Add(new[] { row.Key }.Concat(row.Value.Fields).ToArray());
            }

            var tableValuedParameter = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = SqlDbType.Structured,
                TypeName = tableName,
                Value = table
            };

            return tableValuedParameter;
        }

        private DataSet BuildDataSet(DataTable categoriesTable, DataTable keysTable, int[] keySeverities)
        {
            ICollection<SettingsCategoryInfo> settingsGroups = new List<SettingsCategoryInfo>();
            ICollection<SettingsCategoryInfo> settingsCategories = new List<SettingsCategoryInfo>();

            var categories = categoriesTable
                .Select()
                .Select(r => new SettingsCategoryInfo(r));

            foreach (var category in categories)
            {
                if (category.CategoryLevel > 0)
                {
                    if (category.CategoryIsGroup)
                    {
                        settingsGroups.Add(category);
                    }
                    else
                    {
                        settingsCategories.Add(category);
                    }
                }
            }

            var finalDataSet = new DataSet();

            // This is required because ~\KInspector.Web\FrontEnd\index.html:298 ng-repeat forces an alphabetic order for some reason
            int ngRepeatWorkaround = 0;

            foreach (SettingsCategoryInfo category in settingsCategories)
            {
                var categoryTable = BuildCategoryTable(categoriesTable, keysTable, settingsGroups, category, $"{ngRepeatWorkaround++:00} ", keySeverities);
                finalDataSet.Tables.Add(categoryTable);
            }

            return finalDataSet;
        }

        private DataTable BuildCategoryTable(DataTable categoriesTable, DataTable keysTable, ICollection<SettingsCategoryInfo> settingsGroups, SettingsCategoryInfo category, string tableNamePrefix, int[] keySeverities)
        {
            var categoryPath = category.GetDisplayPath(id => GetDisplayNameFromTable(categoriesTable, $"{nameof(SettingsCategoryInfo.CategoryID)} = {id}", nameof(SettingsCategoryInfo.CategoryDisplayName)));

            var categoryTable = GetCategoryDataTable(tableNamePrefix + categoryPath);

            foreach (var settingsGroup in settingsGroups)
            {
                if (category.CategoryID == settingsGroup.CategoryParentID)
                {
                    var groupPathSegments = settingsGroup.GetIDPathSegments();

                    // Categories in levels 3 and higher are valid groups
                    var keys = keysTable
                        .Select($"{nameof(KeyInfo.KeyCategoryID)} IN ({string.Join(", ", groupPathSegments.Skip(2))})")
                        .Select(r => new KeyInfo(r));

                    foreach (var key in keys)
                    {
                        var groupName = GetDisplayNameFromTable(categoriesTable, $"{nameof(SettingsCategoryInfo.CategoryID)} = {settingsGroup.GetIDPathSegments().Last()}", nameof(SettingsCategoryInfo.CategoryDisplayName));

                        AddKeyToCategoryTable(categoryTable, key, groupName, keySeverities);
                    }
                }
            }

            // Create empty table
            if (categoryTable.Rows.Count == 0)
            {
                categoryTable.Columns.Clear();
                categoryTable.Columns.Add(" ");
                categoryTable.Rows.Clear();
                categoryTable.Rows.Add("No recommendations.");
            }

            return categoryTable;
        }

        private void AddKeyToCategoryTable(DataTable categoryTable, KeyInfo key, string groupName, int[] keySeverities)
        {
            var siteOverrides = key.SiteID > 0 ? "True" : "False";
            var keyDisplayName = $"{GetDisplayName(key.KeyDisplayName)} ({key.KeyName})";
            var keyValue = key.KeyValue ?? string.Empty;

            var keyData = _keyDataDictionary[key.KeyName];

            if (keyData.KeyIsValid == null || !keyData.KeyIsValid(keyValue))
            {
                categoryTable.Rows.Add(new[] {
                                groupName,
                                keyDisplayName,
                                keyValue,
                                key.RecommendedValue,
                                siteOverrides,
                                key.Notes
                            });

                switch (keyData.KeySeverity)
                {
                    case KeySeverity.Critical:
                        keySeverities[2]++;
                        break;

                    case KeySeverity.Warning:
                        keySeverities[1]++;
                        break;

                    case KeySeverity.Placebo:
                        keySeverities[0]++;
                        break;
                }
            }
        }

        private static string GetDisplayNameFromTable(DataTable table, string where, string displayNameColumnName)
        {
            var foundRows = table.Select(where);

            return GetDisplayName(foundRows[0][displayNameColumnName].ToString());
        }

        private static string GetDisplayName(string macrodDisplayName, bool removeMacro = true)
        {
            if (removeMacro)
            {
                macrodDisplayName = macrodDisplayName.Trim(new[] { '{', '$', '}' });
            }

            var localized = _CMSResxSet?.GetString(macrodDisplayName, true);

            return string.IsNullOrEmpty(localized) ? macrodDisplayName : localized;
        }

        private static DataTable GetCategoryDataTable(string tableName)
        {
            var categoryTable = new DataTable(tableName);
            categoryTable.Columns.Add(KeyInfo.DisplayNames[nameof(KeyInfo.GroupName)]);
            categoryTable.Columns.Add(KeyInfo.DisplayNames[nameof(KeyInfo.KeyName)]);
            categoryTable.Columns.Add(KeyInfo.DisplayNames[nameof(KeyInfo.KeyValue)]);
            categoryTable.Columns.Add(KeyInfo.DisplayNames[nameof(KeyInfo.RecommendedValue)]);
            categoryTable.Columns.Add(KeyInfo.DisplayNames[nameof(KeyInfo.SiteOverrides)]);
            categoryTable.Columns.Add(KeyInfo.DisplayNames[nameof(KeyInfo.Notes)]);

            return categoryTable;
        }

        private class KeyData
        {
            private readonly string _recommendedValue;
            private readonly string _notes;

            public Func<string, bool> KeyIsValid { get; }

            public KeySeverity KeySeverity { get; }

            public string[] Fields => new string[] { _recommendedValue, _notes };

            public KeyData(Func<string, bool> keyIsValid, string recommendedValue = null, string notes = null) : this(keyIsValid, KeySeverity.Placebo, recommendedValue, notes)
            {
            }

            public KeyData(Func<string, bool> keyIsValid, KeySeverity keySeverity = KeySeverity.Placebo, string recommendedValue = null, string notes = null)
            {
                KeyIsValid = keyIsValid;
                KeySeverity = keySeverity;
                _recommendedValue = recommendedValue;
                _notes = notes;
            }
        }

        private enum KeySeverity
        {
            Placebo,
            Warning,
            Critical
        }

        private static class Recommended
        {
            public const string NOT_EMPTY = "Not empty";
            public const string EMPTY = "Empty";
            public const string TRUE = "True";
            public const string FALSE = "False";
            public const string CONFIGURED = "Configured";

            public static Func<string, bool> IS_NOT_EMPTY = s => !string.IsNullOrEmpty(s);
            public static Func<string, bool> IS_EMPTY = s => string.IsNullOrEmpty(s);
            public static Func<string, bool> IS_TRUE = s => bool.Parse(s);
            public static Func<string, bool> IS_FALSE = s => !bool.Parse(s);
        }

        private class KeyInfo : SimpleBaseInfo
        {
            public string GroupName => Get<string>(nameof(GroupName));

            public int KeyCategoryID => Get<int>(nameof(KeyCategoryID));

            public int SiteID => Get<int>(nameof(SiteID));

            public bool SiteOverrides => Get<bool>(nameof(SiteOverrides));

            public string KeyName => Get<string>(nameof(KeyName));

            public string KeyDisplayName => Get<string>(nameof(KeyDisplayName));

            public string KeyValue => Get<string>(nameof(KeyValue));

            public string RecommendedValue => Get<string>(nameof(RecommendedValue));

            public string Notes => Get<string>(nameof(Notes));

            public static IDictionary<string, string> DisplayNames = new Dictionary<string, string>{
                { nameof(GroupName), "Settings group"},
                { nameof(KeyCategoryID), "Key category ID"},
                { nameof(SiteID), "Site ID"},
                { nameof(SiteOverrides), "Site overrides"},
                { nameof(KeyName), "Setting"},
                { nameof(KeyValue), "Value"},
                { nameof(RecommendedValue), "Recommended value"},
                { nameof(Notes), "Notes"}
            };

            public KeyInfo(DataRow row) : base(row)
            {
            }
        }

        private class SettingsCategoryInfo : SimpleBaseInfo
        {
            public int CategoryID => Get<int>(nameof(CategoryID));

            public string CategoryIDPath => Get<string>(nameof(CategoryIDPath));

            public bool CategoryIsGroup => Get<bool>(nameof(CategoryIsGroup));

            public string CategoryDisplayName => Get<string>(nameof(CategoryDisplayName));

            public int CategoryLevel => Get<int>(nameof(CategoryLevel));

            public int CategoryParentID => Get<int>(nameof(CategoryParentID));

            public SettingsCategoryInfo(DataRow row) : base(row)
            {
            }

            public string GetDisplayPath(Func<int, string> displayNameFunc)
            {
                var path = GetIDPathSegments().Select(displayNameFunc);

                return string.Join(PATH_SEPARATOR, path);
            }

            public int[] GetIDPathSegments()
            {
                return CategoryIDPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s))
                    .ToArray();
            }
        }
    }
}