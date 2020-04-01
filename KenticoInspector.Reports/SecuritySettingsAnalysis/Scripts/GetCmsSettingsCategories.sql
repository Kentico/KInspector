SELECT 
	CategoryID, 
	CategoryDisplayName 
	
FROM 
	CMS_SettingsCategory 

WHERE 
	CategoryID IN @cmsSettingsCategoryIdsOnPaths