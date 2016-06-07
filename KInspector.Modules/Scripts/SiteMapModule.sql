DECLARE @SiteID int
SELECT @SiteID = MIN(SiteID) FROM [CMS_Site]

WHILE @SiteID is not null
BEGIN
	
	SELECT SiteName AS '#KInspectorNextTableName' FROM [CMS_Site] WHERE SiteID = @SiteID
	
	SELECT 
	  CONCAT(REPLICATE('--', NodeLevel), ' /', DocumentName) AS Document
	  ,[ChildNodesCount]
	  ,[ClassName] AS Class
	  ,[DocumentCulture] AS Culture
	  ,Aliases
	  ,Wildcards
	  ,NULLIF(COUNT(ScopeStartingPath), 0) AS WorkflowScopes
	  ,[DocumentAttachmentCount] AS Attachments
	  ,[ACLS]
	  ,CASE 
			WHEN [IsSecuredNode] = 0 THEN 'NO' 
			WHEN [IsSecuredNode] = 1 THEN 'SECURED' 
			ELSE NULL
	   END AS IsSecured 
	  ,CASE 
			WHEN [RequiresSSL] = 0 THEN 'NO' 
			WHEN [RequiresSSL] = 1 THEN 'REQUIRED'
			WHEN [RequiresSSL] = 2 THEN 'NEVER' 
			ELSE NULL
	   END AS [SSL]
	  ,[NodeCacheMinutes] AS [OutputCache]
	  ,CASE 
			WHEN [NodeAllowCacheInFileSystem] = 0 THEN 'NO' 
			WHEN [NodeAllowCacheInFileSystem] = 1 THEN 'IN FILE' 
			ELSE NULL
	  END AS [CacheInFile]
	  ,[SKUEnabled] AS SKU
	  ,[NodeID]
      ,[NodeParentID]
	  ,CASE
			WHEN [DocumentPageDescription] is null THEN 'EMPTY'
			WHEN [DocumentPageDescription] = '' THEN 'EMPTY'
			ELSE 'SET'
		END AS SEODescription
	  ,CASE
			WHEN [DocumentPageKeyWords] is null THEN 'EMPTY'
			WHEN [DocumentPageKeyWords] = '' THEN 'EMPTY'
			ELSE 'SET'
		END AS SEOKeyWords
	FROM 
		-- Documents with aliases
		(SELECT 
		  NodeLevel
		  ,DocumentName
		  ,[ClassName]
		  ,[DocumentCulture]
		  ,NULLIF( COUNT(CASE
				WHEN [AliasURLPath] IS NULL OR [AliasURLPath] = '' OR [AliasCulture] != [DocumentCulture] THEN NULL
				ELSE 1
			END), 0) AS Aliases
		  ,NULLIF( COUNT(
			CASE
				WHEN [AliasWildcardRule] IS NULL OR [AliasWildcardRule] = '' OR [AliasCulture] != [DocumentCulture] THEN NULL
				ELSE 1
			END
			) + CASE WHEN [DocumentWildcardRule] IS NOT NULL AND [DocumentWildcardRule] != '' THEN 1 ELSE 0 END, 0)  AS Wildcards
		  ,[IsSecuredNode]
		  ,[RequiresSSL]
		  ,[NodeCacheMinutes] 
		  ,[NodeAllowCacheInFileSystem]
		  ,[SKUEnabled]
		  ,[NodeID]
		  ,[NodeParentID]    
		  ,[NodeAliasPath]
		  ,[NodeClassID]
		  ,[DocumentID]
		  ,[NodeACLID]
		  ,[DocumentPageDescription]
		  ,[DocumentPageKeyWords]
		FROM [View_CMS_Tree_Joined]   
	
			-- Document aliases
			LEFT JOIN [CMS_DocumentAlias] 
				ON NodeID = AliasNodeID
		WHERE NodeSiteID = @SiteID 
		GROUP BY NodeID, NodeLevel, DocumentName, ClassName, DocumentCulture, IsSecuredNode, RequiresSSL, NodeCacheMinutes, NodeAllowCacheInFileSystem, SKUEnabled, NodeParentID, NodeAliasPath, DocumentWildcardRule, NodeClassID, DocumentID, NodeACLID, DocumentPageDescription, DocumentPageKeyWords) AS SM 
	
	-- Document attachments
	LEFT JOIN (SELECT [AttachmentDocumentID], COUNT(AttachmentID) AS DocumentAttachmentCount
					FROM [CMS_Attachment] GROUP BY [AttachmentDocumentID] ) AS ATT
		ON DocumentID = ATT.AttachmentDocumentID
	
	-- Child nodes count query
	LEFT JOIN (SELECT NodeID as NID, ISNULL(C.Count, 0) as ChildNodesCount 
				FROM CMS_Tree AS Data 
					LEFT JOIN (SELECT NodeParentID, count(*) Count FROM CMS_Tree GROUP BY NodeParentID) AS C
					ON NodeID = C.NodeParentID) AS ChildNodes
				ON NodeID = ChildNodes.NID

	-- Document ACLS
	LEFT JOIN (SELECT NodeID AS ACLNodeID, COUNT(Operator) AS ACLS FROM View_CMS_ACLItem_ItemsAndOperators JOIN CMS_Tree ON ACLID IN (SELECT r.value('.','VARCHAR(MAX)') as ACLID FROM (SELECT CONVERT(XML, N'<root><r>' + REPLACE((SELECT  ACLInheritedACLs FROM CMS_ACL WHERE ACLID = NodeACLID) , ',', '</r><r>') + '</r></root>') as valxml) x CROSS APPLY x.valxml.nodes('//root/r') AS RECORDS(r) UNION SELECT NodeACLID) WHERE NodeSiteID = @SiteID GROUP BY NodeID) AS A ON SM.NodeID = A.ACLNodeID
	
	-- Document workflow scopes
	LEFT JOIN (SELECT ScopeClassID, ScopeStartingPath, ScopeExcludeChildren, CultureCode AS ScopeCultureCode
					FROM CMS_WorkflowScope LEFT JOIN CMS_Culture ON ScopeCultureID = CultureID 
					WHERE ScopeSiteID = @SiteID) AS WF 
		ON (ScopeClassID IS NULL OR ScopeClassID = SM.NodeClassID)
			AND (ScopeCultureCode IS NULL OR ScopeCultureCode = DocumentCulture)
			AND ((ScopeStartingPath = '/' OR NodeAliasPath LIKE ScopeStartingPath + '/%' OR NodeAliasPath LIKE ScopeStartingPath) AND 
			-- Special case for root				
			(ScopeStartingPath <> '/%' OR NodeAliasPath <> '/') AND
			-- Do not select scope with excluded children unless it is the node itself
			(ScopeExcludeChildren <> 1 OR ScopeExcludeChildren IS NULL OR ScopeStartingPath = NodeAliasPath))
	GROUP BY NodeLevel, DocumentName, ChildNodesCount, [ClassName], [DocumentCulture], Aliases, Wildcards,[IsSecuredNode],[RequiresSSL],[NodeCacheMinutes] ,[NodeAllowCacheInFileSystem],[SKUEnabled],[NodeID],[NodeParentID],[NodeAliasPath],[NodeClassID],[DocumentID], [DocumentAttachmentCount], [ACLS], [DocumentPageDescription], [DocumentPageKeyWords]
	ORDER BY DocumentCulture, NodeAliasPath;
  
  SELECT @SiteID = MIN(SiteID) FROM [CMS_Site] where SiteID > @SiteID
END
