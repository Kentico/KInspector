-- Get  settings 
SELECT KeyName, KeyValue, '' as Notes
FROM CMS_SettingsKey
WHERE (KeyName IN ('CMSFloodProtectionEnabled', 'CMSChatEnableFloodProtection'))
