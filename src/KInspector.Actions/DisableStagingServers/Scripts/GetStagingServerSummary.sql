SELECT C.ServerID AS 'ID', C.ServerDisplayName AS 'Name', C.ServerEnabled AS 'Enabled', C.ServerURL AS 'Url', S.SiteDisplayName AS 'Site'
FROM Staging_Server AS C
JOIN CMS_Site AS S ON S.SiteID = C.ServerSiteID