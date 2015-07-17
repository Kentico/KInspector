-- Web farm modification
------------------------
-- Web farm server is disabled by setting its Enabled state to diasbled
-- and appending '.disabled' suffix to display name so that it is possible
-- to distinguish servers disabled by the customer and by audit.

-- Select Web farm servers which are disabled in the DB provided for audit
SELECT N'1a Web farm servers which were disabled by the customer' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_WebFarmServer]
WHERE ServerEnabled = 0 AND ServerDisplayName NOT LIKE N'%.disabled'

-- Select Web farm servers which were disabled by previous setup run
SELECT N'1b Web farm servers which were disabled by previous Web farm setup' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_WebFarmServer]
WHERE ServerEnabled = 0 AND ServerDisplayName LIKE N'%.disabled'

-- Select Web farm servers which will be disabled by this setup run
SELECT N'1c Web farm servers which were disabled by this Web farm setup' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_WebFarmServer]
WHERE ServerEnabled = 1

-- Disable necessary Web farm servers
UPDATE [CMS_WebFarmServer] SET ServerDisplayName = ServerDisplayName + N'.disabled', ServerEnabled = 0
WHERE ServerEnabled = 1
