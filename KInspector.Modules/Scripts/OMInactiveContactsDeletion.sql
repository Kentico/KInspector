SELECT KeyName as 'Key', COALESCE(KeyValue, 'Not set') as 'Value', COALESCE(SiteName, 'Global') as 'Site'
FROM CMS_SettingsKey 
LEFT JOIN CMS_Site ON CMS_Site.SiteID = CMS_SettingsKey.SiteID
WHERE KeyCategoryID IN
	(SELECT CategoryID FROM CMS_SettingsCategory WHERE CategoryName LIKE 'CMS.OnlineMarketing.InactiveContacts%')
	AND 
	EXISTS(SELECT SiteID FROM CMS_SettingsKey WHERE KeyName LIKE 'CMSEnableOnlineMarketing' 
			AND KeyValue = 'True'
			AND COALESCE(SiteID, 0) = COALESCE(CMS_SettingsKey.SiteID, 0))