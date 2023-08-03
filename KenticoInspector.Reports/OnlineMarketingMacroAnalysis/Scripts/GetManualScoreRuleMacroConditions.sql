SELECT S.ScoreDisplayName AS 'ScoreName', R.RuleDisplayName AS 'RuleName'
FROM OM_Score AS S
JOIN OM_Rule AS R ON S.ScoreID = R.RuleScoreID
WHERE R.RuleType = 2 AND R.RuleCondition NOT LIKE '%{%%Rule(%'