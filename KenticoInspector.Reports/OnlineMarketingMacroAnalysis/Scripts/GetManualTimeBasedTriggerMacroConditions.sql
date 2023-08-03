SELECT W.WorkflowDisplayName AS 'ProcessName', T.TriggerDisplayName 'TriggerName'
FROM CMS_Workflow AS W
JOIN CMS_ObjectWorkflowTrigger AS T ON T.TriggerWorkflowID = W.WorkflowID
WHERE W.WorkflowType = 3 AND T.TriggerType = 2 AND T.TriggerMacroCondition IS NOT NULL AND T.TriggerMacroCondition NOT LIKE '{%%Rule(%'