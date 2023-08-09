SELECT ServerID AS 'ID', ServerName AS 'Name',
       CASE WHEN ServerEnabled = 1 THEN 'true' ELSE 'false' END AS 'Enabled'
FROM CMS_WebFarmServer