
DECLARE @SiteDomainAliasName AS NVARCHAR(400) = N'localhost'

DECLARE @SiteID AS INT
DECLARE @SiteName AS NVARCHAR(100)

-- Delete any previous aliases
DELETE FROM CMS_SiteDomainAlias WHERE SiteDomainAliasName = 'localhost'

-- Add new aliases
DECLARE sitesCursor CURSOR LOCAL FAST_FORWARD FOR
SELECT SiteID, SiteName FROM [CMS_Site]

OPEN sitesCursor
FETCH NEXT FROM sitesCursor INTO @SiteID, @SiteName
WHILE @@FETCH_STATUS = 0
BEGIN
INSERT INTO [CMS_SiteDomainAlias] (SiteDomainAliasName, SiteID, SiteDefaultVisitorCulture, SiteDomainGUID, SiteDomainLastModified, SiteDomainDefaultAliasPath, SiteDomainRedirectUrl)
VALUES (@SiteDomainAliasName, @SiteID, N'', NEWID(), CURRENT_TIMESTAMP, N'', N'')

PRINT @SiteName + N' (ID: ' + CAST(@SiteID AS NVARCHAR(8)) + N') has been added the alias ' + @SiteDomainAliasName

FETCH NEXT FROM sitesCursor INTO @SiteID, @SiteName
END
CLOSE sitesCursor
DEALLOCATE sitesCursor