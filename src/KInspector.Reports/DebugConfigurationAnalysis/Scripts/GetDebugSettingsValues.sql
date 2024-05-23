SELECT 
	KeyName, KeyDisplayName, KeyValue, KeyDefaultValue
FROM
	CMS_SettingsKey 
WHERE
	KeyName = 'CMSDisableDebug' OR
	( 
		KeyType = 'boolean' and -- Get only boolean-type settings 
		KeyName like 'CMSDebug%' and -- Get only debug-related settings
		KeyName not like '%all%' and -- Filters out settings to also debug in the admin
		KeyName not like '%live%' and -- Filters out settings to display debug information on the live site
		KeyName not like '%stack%' and -- Filters out settings to display stack information
		KeyName != 'CMSDebugMacrosDetailed' -- Filters out setting for showing more details for macro debugging
	)