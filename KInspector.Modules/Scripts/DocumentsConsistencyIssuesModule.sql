-- Nodes with NodeSiteID missing
SELECT 'CMS_Tree with NodeSiteID missing (Missing parent site)' AS '#KInspectorNextTableName'
SELECT NodeID, NodeAliasPath
FROM CMS_Tree 
WHERE NodeSiteID IS NULL
OR NodeSiteID = 0
OR NodeSiteID NOT IN (SELECT SiteID FROM CMS_Site)


-- Nodes where parent node is missing
SELECT 'Nodes where parent node is missing' AS '#KInspectorNextTableName'
SELECT NodeID, NodeAliasPath, NodeParentID, NodeSiteID 
FROM CMS_Tree as Child
WHERE NodeParentID NOT IN (Select NodeID from CMS_Tree)
AND NodeParentID != 0


-- Documents where related node is missing
SELECT 'Related node is missing - specified DocumentNodeID is pointing to non-existing node.' AS '#KInspectorNextTableName'
SELECT DocumentID, DocumentNamePath, DocumentNodeID 
FROM CMS_Document
WHERE DocumentNodeID NOT IN (SELECT NodeID FROM CMS_Tree)


-- Child nodes count issues
SELECT 'Nodes where parent NodeLevel is not one level smaller (NodeLevel - 1)' AS '#KInspectorNextTableName'
SELECT Child.NodeID, Child.NodeLevel, Child.NodeAliasPath, 
	Parent.NodeID as 'ParentNodeID', Parent.NodeLevel as 'ParentNodeLevel', Parent.NodeAliasPath as 'ParentNodeAliasPath' 
FROM CMS_Tree as Child
LEFT JOIN CMS_Tree as Parent on Child.NodeParentID = Parent.NodeID
WHERE (Child.NodeLevel - 1) != Parent.NodeLevel

SELECT 'Nodes where NodeLevel doesn''t correspond to number of ''/'' in NodeAliasPath' AS '#KInspectorNextTableName'
SELECT NodeID, NodeLevel, NodeAliasPath
FROM CMS_Tree
WHERE NodeLevel != LEN(NodeAliasPath) - LEN(REPLACE(NodeAliasPath,'/','')) -- NodeLevel = number of '/' in it
and NodeLevel != 0 and NodeParentID != 0 -- Root nodes


-- Missing CMS_Document entries
SELECT 'Missing CMS_Document entries (CMS_Tree without CMS_Document representation)' AS '#KInspectorNextTableName'
SELECT NodeID, NodeAliasPath, NodeSiteID
FROM CMS_Tree
WHERE NodeID not in (SELECT DocumentNodeID from CMS_Document)
and NodeLinkedNodeID != null


-- Duplicate CMS_Tree entries
SELECT 'Duplicate CMS_Tree entries (CMS_Tree without CMS_Document representation)' AS '#KInspectorNextTableName'
SELECT Original.NodeID, Original.NodeAliasPath, Original.NodeSiteID,
	   Duplicate.NodeID as 'DuplicateNodeID', Duplicate.NodeAliasPath as 'DuplicateNodeAliasPath', Duplicate.NodeSiteID as 'DuplicateNodeSiteID'
FROM CMS_Tree as Original
LEFT JOIN CMS_Tree as Duplicate on Original.NodeAliasPath = Duplicate.NodeAliasPath 
WHERE Original.NodeID != Duplicate.NodeID
AND Original.NodeSiteID = Duplicate.NodeSiteID
ORDER BY Original.NodeAliasPath


-- Document type is not assigned to site, but it is used by the web site.
SELECT 'Document type is not assigned to site, but it is used by the web site.' AS '#KInspectorNextTableName'
SELECT ClassName, SiteName FROM
(SELECT NodeClassID, NodeSiteID
FROM CMS_Tree
WHERE NOT EXISTS (SELECT * 
				  FROM  CMS_ClassSite
				  WHERE ClassID = NodeClassID and SiteID = NodeSiteID)
GROUP BY NodeClassID, NodeSiteID) as MissingBinding
LEFT JOIN CMS_Class on MissingBinding.NodeClassID = ClassID
LEFT JOIN CMS_Site on MissingBinding.NodeSiteID = SiteID
