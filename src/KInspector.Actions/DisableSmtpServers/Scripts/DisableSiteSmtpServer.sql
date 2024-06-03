IF @SiteID = 0 BEGIN
	UPDATE CMS_SettingsKey SET KeyValue = KeyValue + '.disabled' WHERE KeyName = N'CMSSMTPServer' AND SiteID IS NULL
END
ELSE BEGIN
	UPDATE CMS_SettingsKey SET KeyValue = KeyValue + '.disabled' WHERE KeyName = N'CMSSMTPServer' AND SiteID = @SiteID
END