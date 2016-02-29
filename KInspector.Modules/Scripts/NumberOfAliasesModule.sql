select count(DocumentID) as DocumentCount from CMS_Document;

select count(AliasID) as AliasCount from CMS_DocumentAlias;

select count(AliasID) as AliasCount, AliasNodeID, NodeAliasPath from CMS_DocumentAlias join CMS_Tree on CMS_DocumentAlias.AliasNodeID = CMS_Tree.NodeID  group by AliasNodeID, NodeAliasPath order by AliasCount desc;

