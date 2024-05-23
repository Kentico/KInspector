SELECT W.WorkflowDisplayName AS 'ProcessName', T.TriggerDisplayName AS 'TriggerName', T.TriggerMacroCondition AS 'Macro'
FROM CMS_Workflow AS W
JOIN CMS_ObjectWorkflowTrigger AS T ON T.TriggerWorkflowID = W.WorkflowID
WHERE W.WorkflowType = 3 AND T.TriggerType = 2 AND T.TriggerMacroCondition IS NOT NULL AND T.TriggerMacroCondition NOT LIKE '{%%Rule(%'