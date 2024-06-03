UPDATE Staging_Server SET ServerDisplayName = ServerDisplayName + N'.disabled', ServerEnabled = 0
WHERE ServerID = @ServerID