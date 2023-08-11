DECLARE @SettingsTable TABLE (
	KeyName nvarchar(max),
	KeyValue nvarchar(max),
	SiteID int
)

INSERT INTO @SettingsTable SELECT KeyName, KeyValue, SiteID FROM CMS_SettingsKey WHERE KeyName IN (N'CMSSMTPServer', N'CMSSMTPServerUser', N'CMSSMTPServerAuthenticationType')

SELECT S.SiteID AS 'SiteID', S.SiteDisplayName AS 'SiteName',
	   (SELECT KeyValue FROM @SettingsTable WHERE KeyName = N'CMSSMTPServer' AND SiteID = S.SiteID) AS 'Server',
	   (SELECT KeyValue FROM @SettingsTable WHERE KeyName = N'CMSSMTPServerUser' AND SiteID = S.SiteID) AS 'User',
	   (SELECT KeyValue FROM @SettingsTable WHERE KeyName = N'CMSSMTPServerAuthenticationType' AND SiteID = S.SiteID) AS 'Authentication'
FROM CMS_Site S
UNION
SELECT 0 AS 'SiteID', '(global)' AS 'SiteName',
	   (SELECT KeyValue FROM @SettingsTable WHERE KeyName = N'CMSSMTPServer' AND SiteID IS NULL) AS 'Server',
	   (SELECT KeyValue FROM @SettingsTable WHERE KeyName = N'CMSSMTPServerUser' AND SiteID IS NULL) AS 'User',
	   (SELECT KeyValue FROM @SettingsTable WHERE KeyName = N'CMSSMTPServerAuthenticationType' AND SiteID IS NULL) AS 'Authentication'