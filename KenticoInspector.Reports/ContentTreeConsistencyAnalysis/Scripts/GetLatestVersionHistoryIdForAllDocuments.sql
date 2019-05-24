SELECT LatestVersionHistoryID
	FROM
	(
		SELECT MAX(VersionHistoryID) OVER (PARTITION BY DocumentID) AS LatestVersionHistoryID
			FROM CMS_VersionHistory
	) AS LatestVersionHistoryIDs
	GROUP BY LatestVersionHistoryID