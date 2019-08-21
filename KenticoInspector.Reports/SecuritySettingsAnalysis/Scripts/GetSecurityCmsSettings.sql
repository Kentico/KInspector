SELECT 
	SiteID,
	KeyID,
	KeyName,
    KeyDisplayName,
	KeyValue,
    KeyDefaultValue,
	KeyCategoryID,
	CategoryIDPath

FROM 
	CMS_SettingsKey S

LEFT JOIN CMS_SettingsCategory C ON C.CategoryIDPath LIKE CONCAT('%', S.KeyCategoryID)

WHERE 
	KeyName IN @cmsSettingsKeysNames