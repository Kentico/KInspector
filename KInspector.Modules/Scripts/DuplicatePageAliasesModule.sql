SELECT DISTINCT x.AliasURLPath, x.Repetitions, x.AffectedNodeIDs as 'Affected Node IDs', x.Reason
FROM
(
	SELECT DISTINCT AliasURLPath, Repetitions, CAST(AliasNodeID AS VARCHAR(20)) AS AffectedNodeIDs, Reason
	FROM (
			SELECT a1.AliasURLPath, a1.AliasNodeID, COUNT(*) as Repetitions, 'AliasURLPath has been assigned multiple times to the same NodeID.' as Reason 
		FROM CMS_DocumentAlias a1
		INNER JOIN CMS_DocumentAlias a2
		ON a1.AliasURLPath = a2.AliasURLPath
		AND a1.AliasNodeID = a2.AliasNodeID
		AND (a1.AliasCulture = a2.AliasCulture OR a1.AliasCulture = '' OR a2.AliasCulture = '')
		AND (a1.AliasID <> a2.AliasID)
		GROUP BY a1.AliasURLPath, a1.AliasNodeID
	) a3
	WHERE Repetitions > 1

	UNION
	SELECT DISTINCT b2.AliasURLPath, b2.Repetitions, b6.UsedNodes AS AffectedNodeIDs, b2.Reason
	FROM (
		SELECT AliasURLPath, AliasCulture, COUNT(*) as Repetitions, 'AliasURLPath has been assigned multiple times to different NodeIDs.' as Reason 
		FROM 
			( SELECT DISTINCT AliasURLPath, AliasCulture, AliasNodeID FROM CMS_DocumentAlias) b1
		GROUP BY AliasURLPath, AliasCulture
	) b2
	INNER JOIN
	(    Select distinct b3.AliasURLPath, b3.AliasCulture,
		substring(
			(
				Select ', ' + CAST(b4.AliasNodeID AS VARCHAR(20)) AS [text()]
				From dbo.CMS_DocumentAlias b4
				Where b4.AliasURLPath = b3.AliasURLPath
				AND (b4.AliasCulture = b3.AliasCulture OR b4.AliasCulture = '' OR b3.AliasCulture = '')
				AND (b4.AliasID <> b3.AliasID)
				ORDER BY b4.AliasNodeID
				For XML PATH ('')
			), 3, 1000) UsedNodes
	From dbo.CMS_DocumentAlias b3
	) b6
	ON b2.AliasURLPath = b6.AliasURLPath
	AND (b2.AliasCulture = b6.AliasCulture OR b2.AliasCulture = '' OR b6.AliasCulture = '')
	WHERE Repetitions > 1
	
	UNION
	SELECT DISTINCT AliasURLPath, Repetitions, CAST(AliasNodeID AS VARCHAR(20)) AS AffectedNodeIDs, Reason
	FROM (
		SELECT AliasURLPath, AliasNodeID, COUNT(*) as Repetitions, 'AliasURLPath is used as a NodeAliasPath for NodeID ' + CAST(t.NodeID AS VARCHAR(20)) + '.' as Reason 
		FROM CMS_DocumentAlias d
		INNER JOIN CMS_Tree t
		ON t.NodeAliasPath = d.AliasURLPath
		AND t.NodeID <> d.AliasNodeID
		GROUP BY d.AliasURLPath, d.AliasNodeID, t.NodeID
	) c1
) x
ORDER BY AliasURLPath