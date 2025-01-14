SELECT SiteID AS 'ID', SiteDisplayName AS 'DisplayName', SiteDescription AS 'Description', SiteDomainName AS 'AdministrationDomain',  SitePresentationUrl AS 'PresentationUrl',
       CASE WHEN SiteStatus = 'RUNNING' THEN 1 ELSE 0 END AS 'Running'
FROM CMS_Site