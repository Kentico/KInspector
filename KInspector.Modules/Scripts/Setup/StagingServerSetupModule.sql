-- Staging servers modification
-------------------------------
-- Staging server is disabled by appending its URL with '.disabled'
-- so that it is possible to distinguish servers disabled by the customer and by audit.
-- The Enabled state of the server is not modified so the UI is still fully accessible.

-- Select Staging servers which are disabled in the DB provided for audit
SELECT N'1a Staging servers which were disabled by the customer' AS N'#KInspectorNextTableName'
SELECT * FROM [Staging_Server]
WHERE ServerEnabled = 0

-- Select Staging servers which were disabled by previous setup run
SELECT N'1b Staging servers which were disabled by previous Staging setup' AS N'#KInspectorNextTableName'
SELECT * FROM [Staging_Server]
WHERE ServerEnabled = 1 AND ServerDisplayName LIKE N'%.disabled'

-- Select Staging servers which will be disabled by this setup run
SELECT N'1c Staging servers which were disabled by this Staging setup' AS N'#KInspectorNextTableName'
SELECT * FROM [Staging_Server]
WHERE ServerEnabled = 1 AND ServerDisplayName NOT LIKE N'%.disabled'

-- Disable necessary Staging servers
UPDATE [Staging_Server] SET ServerDisplayName = ServerDisplayName + N'.disabled' WHERE ServerEnabled = 1 AND ServerDisplayName NOT LIKE N'%.disabled'