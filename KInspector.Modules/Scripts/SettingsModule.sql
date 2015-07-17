CREATE TABLE #MyTempTable
    (SiteName nvarchar(MAX),
     KeyName nvarchar(MAX),
     KeyValue nvarchar(MAX),
     FromGlobal nvarchar(MAX)
    );

INSERT INTO #MyTempTable
			SELECT SiteName, KeyName, KeyValue, 'no'
				FROM CMS_SettingsKey LEFT JOIN CMS_Site ON 
				(CMS_SettingsKey.SiteID = CMS_Site.SiteID)
				WHERE KeyValue IS NOT NULL AND SiteName IS NOT NULL

INSERT INTO #MyTempTable 
			SELECT SiteName, KeyName, GlobalSettings.KeyValue, 'yes'
				FROM CMS_Site, CMS_SettingsKey as GlobalSettings
				WHERE GlobalSettings.SiteID IS NULL
				AND NOT EXISTS 
					(SELECT KeyValue FROM CMS_SettingsKey as SiteSettings WHERE SiteSettings.KeyName = GlobalSettings.KeyName AND SiteSettings.SiteID IS NOT NULL)
				OR EXISTS
					(SELECT KeyValue FROM CMS_SettingsKey as SiteSettings WHERE SiteSettings.KeyName = GlobalSettings.KeyName AND SiteSettings.KeyValue IS NULL AND SiteSettings.SiteID IS NOT NULL)

SELECT SiteName as 'Site name', KeyName as 'Key name', KeyValue as 'Key value', FromGlobal as 'Inherited from global' FROM #MyTempTable
WHERE KeyName IN (
					'CMSStoreFilesInFileSystem',
					'CMSStoreFilesInDatabase',
					'CMSGenerateThumbnails',
					'CMSFilesFolder',
					'CMSBizFormFilesFolder',
					'CMSExcludedFormFilterURLs',
					'CMSExcludedResolveFilterURLs',
					'CMSLogSize',
					'CMSLogMetadata',
					'CMSStagingLogChanges',
					'CMSStagingLogDataChanges',
					'CMSStagingLogObjectChanges',
					'CMSStagingLogStagingChanges',
					'CMSExportLogObjectChanges',
					'CMSMediaUsePermanentURLs',
					'CMSAnalyticsEnabled',
					'CMSCachePageInfo',
					'CMSCacheMinutes',
					'CMSProgressiveCaching',
					'CMSClientCacheMinutes',
					'CMSRevalidateClientCache',
					'CMSSearchIndexingEnabled',
					'CMSCheckPublishedFiles',
					'CMSCheckFilesPermissions',
					'CMSCacheImages',
					'CMSMaxCacheFileSize',
					'CMSRedirectFilesToDisk',
					'CMSUseSessionManagement',
					'CMSSessionUseDBRepository',
					'CMSEmailQueueEnabled',
					'CMSCheckMediaFilePermissions',
					'CMSMediaLibrariesFolder',
					'CMSExcludedXHTMLFilterURLs',
					'CMSExcludedAttributesFilterURLs',
					'CMSExcludedJavascriptFilterURLs',
					'CMSExcludedLowercaseFilterURLs',
					'CMSExcludedSelfcloseFilterURLs',
					'CMSExcludedTagsFilterURLs',
					'CMSExcludedHTML5FilterURLs',
					'CMSConvertTablesToDivs',
					'CMSIndentOutputHtml',
					'CMSUsePermanentURLs',
					'CMSImageWatermark',
					'CMSContentImageWatermark',
					'CMSMediaImageWatermark',
					'CMSMetaImageWatermark',
					'CMSScriptMinificationEnabled',
					'CMSStylesheetMinificationEnabled',
					'CMSEnableHealthMonitoring',
					'CMSUseExternalService',
					'CMSEnableOnlineMarketing',
					'CMSSchedulerInterval',
					'CMSSchedulerUseExternalService',
					'CMSSchedulerServiceInterval',
					'CMSLogPageNotFoundException',
					'CMSResolveMacrosInCSS',
					'CMSAllowComponentsCSS',
					'CMSCombineComponentsCSS',
					'CMSDisableDebug',
					'CMSEnableOutputCache',
					'CMSFileSystemOutputCacheMinutes',
					'CMSEnablePartialCache',
					'CMSAllowGZip',
					'CMSUseLangPrefixForUrls',
					'CMSAllowUrlsWithoutLanguagePrefixes',
					'CMSUseURLsWithTrailingSlash',
					'CMSRedirectAliasesToMainURL',
					'CMSRedirectInvalidCasePages',
					'CMSRedirectToMainExtension',
					'CMSProcessDomainPrefix',
					'CMSDefaulPage',
					'CMSWebAnalyticsUseJavascriptLogging',
					'CMSExcludedURLs'
				) ORDER BY SiteName, KeyName
DROP TABLE #MyTempTable