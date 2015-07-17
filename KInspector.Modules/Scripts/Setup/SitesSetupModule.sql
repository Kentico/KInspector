-- Stops all running sites
--------------------------

SELECT N'1 Sites stopped by this setup' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_Site] WHERE SiteStatus != N'STOPPED'

SELECT N'2 Sites already stopped' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_Site] WHERE SiteStatus = N'STOPPED'

UPDATE [CMS_Site] SET SiteStatus = N'STOPPED' WHERE SiteStatus != N'STOPPED'