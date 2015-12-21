SELECT x.AliasURLPath, x.AliasCulture, x.Repetitions, x.AffectedNodeIDs, x.Reason
FROM
(
	SELECT DISTINCT AliasURLPath, AliasCulture, Repetitions, CAST(AliasNodeID AS VARCHAR(20)) AS AffectedNodeIDs, Reason
	FROM (
		SELECT AliasURLPath, AliasCulture, AliasNodeID, COUNT(*) as Repetitions, 'AliasURLPath has been assigned multiple times to the same NodeID.' as Reason 
		FROM CMS_DocumentAlias
		GROUP BY AliasURLPath, AliasCulture, AliasNodeID
	) a1
	WHERE Repetitions > 1

	UNION
	SELECT DISTINCT b2.AliasURLPath, b2.AliasCulture, b2.Repetitions, b6.UsedNodes AS AffectedNodeIDs, b2.Reason
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
				AND b4.AliasCulture = b3.AliasCulture
				ORDER BY b4.AliasNodeID
				For XML PATH ('')
			), 3, 1000) UsedNodes
	From dbo.CMS_DocumentAlias b3
	) b6
	ON b2.AliasURLPath = b6.AliasURLPath
	AND b2.AliasCulture = b6.AliasCulture
	WHERE Repetitions > 1
	
	UNION
	SELECT DISTINCT AliasURLPath, AliasCulture, Repetitions, CAST(AliasNodeID AS VARCHAR(20)) AS AffectedNodeIDs, Reason
	FROM (
		SELECT AliasURLPath, AliasCulture, AliasNodeID, COUNT(*) as Repetitions, 'AliasURLPath is used as a NodeAliasPath for a different NodeID.' as Reason 
		FROM CMS_DocumentAlias d
		INNER JOIN CMS_Tree t
		ON t.NodeAliasPath = d.AliasURLPath
		AND t.NodeID <> d.AliasNodeID
		GROUP BY AliasURLPath, AliasCulture, AliasNodeID
	) c1
) x
ORDER BY AliasURLPath