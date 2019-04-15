-- In the case of "CSMUsePasswordPolicy", the setting is given per site name. For that reason
-- the data has to be retrieved for each site so that the PasswordPolicy module can evaluate
-- said value at the 'site' level.
SELECT 
	ISNULL(CMS_Site.SiteDisplayName,'N/A') as SiteDisplayName, 
	CMS_SettingsKey.KeyName, 
	CMS_SettingsKey.KeyValue
FROM CMS_SettingsKey 
LEFT OUTER JOIN CMS_Site ON CMS_SettingsKey.SiteID = CMS_Site.SiteID
WHERE 
	(CMS_SettingsKey.KeyName IN ('CMSPasswordFormat')) 
