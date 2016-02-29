select count(DocumentID) as DocumentCount from CMS_Document;

select count(AliasID) as AliasCount from CMS_DocumentAlias;

select count(AliasID) as AliasCount, AliasNodeID from CMS_DocumentAlias  group by AliasNodeID order by AliasCount desc;

