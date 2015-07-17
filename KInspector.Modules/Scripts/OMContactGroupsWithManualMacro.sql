SELECT ContactGroupDisplayName as 'Contact group name', ContactGroupDynamicCondition as Macro 
FROM OM_ContactGroup 
WHERE ContactGroupDynamicCondition IS NOT NULL AND ContactGroupDynamicCondition NOT LIKE '{%%Rule%'