SELECT        COALESCE (s.SiteDisplayName, 'N/A') AS 'Site Name', sk.KeyName, sk.KeyValue, 'It is recommended that you have CMSFloodProtectionEnabled set to True. You can find this setting here: Security & Membership > Protection > Flood protection' AS Notes
FROM            CMS_SettingsKey AS sk LEFT OUTER JOIN
                         CMS_Site AS s ON sk.SiteID = s.SiteID
WHERE        (sk.KeyName = 'CMSFloodProtectionEnabled') AND (sk.KeyValue = 'FALSE')
UNION
SELECT        COALESCE (s.SiteDisplayName, 'N/A') AS 'Site Name', sk.KeyName, sk.KeyValue, 'It is recommended that you have CMSChatEnableFloodProtection set to True. You can find this setting here: Community > Chat > Flood protection' AS Notes
FROM            CMS_SettingsKey AS sk LEFT OUTER JOIN
                         CMS_Site AS s ON sk.SiteID = s.SiteID
WHERE        (sk.KeyName = 'CMSChatEnableFloodProtection') AND (sk.KeyValue = 'FALSE')