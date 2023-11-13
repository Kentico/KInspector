SELECT ContactGroupDisplayName AS 'ContactGroup', ContactGroupDynamicCondition AS 'Macro'
FROM OM_ContactGroup
WHERE ContactGroupDynamicCondition IS NOT NULL AND ContactGroupDynamicCondition NOT LIKE '{%%Rule(%'