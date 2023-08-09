SELECT SiteID AS 'ID', SiteDisplayName AS 'DisplayName', SiteDescription AS 'Description', SiteDomainName AS 'AdministrationDomain',  SitePresentationUrl AS 'PresentationUrl',
       CASE WHEN SiteStatus = 'RUNNING' THEN 'true' ELSE 'false' END AS 'IsRunning'
FROM CMS_Site