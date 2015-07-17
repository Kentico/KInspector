-- Settings modification
------------------------
-- Defaul email server in Settings is disabled by settings its erver name to invalid value
-- The servers disabled by audit can be identified by having '.disabled' suffix

SELECT N'1a Settings > System > E-mails servers which were not set by the customer' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_SettingsKey] WHERE KeyName = N'CMSSMTPServer' AND (KeyValue IS NULL OR KeyValue = N'')

SELECT N'1b Settings > System > E-mails servers which were disabled by previous setup run' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_SettingsKey] WHERE KeyName = N'CMSSMTPServer' AND KeyValue LIKE N'%.disabled'

SELECT N'1c Settings > System > E-mails servers which were disabled by this setup run' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_SettingsKey] WHERE KeyName = N'CMSSMTPServer' AND KeyValue NOT LIKE N'%.disabled' AND KeyValue IS NOT NULL AND KeyValue != N''
UPDATE [CMS_SettingsKey] SET KeyValue = KeyValue + N'.disabled' WHERE KeyName = N'CMSSMTPServer' AND KeyValue NOT LIKE N'%.disabled' AND KeyValue IS NOT NULL AND KeyValue != N''


-- SMTP servers modification
----------------------------
-- SMTP server is disabled by setting its server name to invalid value
-- The Enabled status is kept untouched so that it is possible
-- to distinguish servers disabled by the customer and by audit.

-- Select SMTP servers which are disabled in the DB provided for audit
SELECT N'2a SMTP servers which were disabled by the customer' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_SMTPServer]
WHERE ServerEnabled = 0

-- Select SMTP servers which were disabled by previous setup run
SELECT N'2b SMTP servers which were disabled by previous SMTP setup' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_SMTPServer]
WHERE ServerEnabled = 1 AND ServerName LIKE N'%.disabled'

-- Select SMTP servers which will be disabled by this setup run
SELECT N'2c SMTP servers which were disabled by this SMTP setup' AS N'#KInspectorNextTableName'
SELECT * FROM [CMS_SMTPServer]
WHERE ServerEnabled = 1 AND ServerName NOT LIKE N'%.disabled'

-- Disable necessary SMTP servers
UPDATE [CMS_SMTPServer]
SET ServerName = ServerName + N'.disabled'
WHERE ServerEnabled = 1 AND ServerName NOT LIKE N'%.disabled'