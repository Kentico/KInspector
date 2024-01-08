SELECT LatestVersionHistoryID
	FROM
	(
		SELECT MAX(VersionHistoryID) OVER (PARTITION BY DocumentID) AS LatestVersionHistoryID
			FROM CMS_VersionHistory
			WHERE WasPublishedFrom IS NOT NULL
	) AS LatestVersionHistoryIDs
	GROUP BY LatestVersionHistoryID