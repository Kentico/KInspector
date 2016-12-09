SELECT KeyName AS 'Key',KeyValue AS 'Value'
FROM CMS_SettingsKey 
WHERE KeyName = 'CMSDisableDebug' OR
  ( 
    KeyName LIKE 'CMSDebug%' and
    KeyType = 'boolean' and
    KeyName not like '%live%' and
    KeyName not like '%stack%' and
    KeyName != 'CMSDebugMacrosDetailed' and
    KeyName not like '%all%'
  )