SELECT ContactGroupDisplayName AS 'ContactGroup'
FROM OM_ContactGroup
WHERE ContactGroupDynamicCondition IS NOT NULL AND ContactGroupDynamicCondition NOT LIKE '{%%Rule(%'