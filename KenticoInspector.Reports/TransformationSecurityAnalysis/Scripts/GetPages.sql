SELECT DISTINCT
    NodeID,
    DocumentName,
    DocumentCulture,
    NodeAliasPath,
    NodeSiteID,
    DocumentPageTemplateID
    
    FROM View_CMS_Tree_Joined 
    
    WHERE
        DocumentPageTemplateID IS NOT NULL
        AND NodeLinkedNodeID IS NULL