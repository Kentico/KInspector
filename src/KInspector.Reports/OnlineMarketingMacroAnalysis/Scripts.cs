namespace KInspector.Reports.OnlineMarketingMacroAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory => $"{nameof(OnlineMarketingMacroAnalysis)}/Scripts";

        public static string GetManualContactGroupMacroConditions => $"{BaseDirectory}/{nameof(GetManualContactGroupMacroConditions)}.sql";

        public static string GetManualTimeBasedTriggerMacroConditions => $"{BaseDirectory}/{nameof(GetManualTimeBasedTriggerMacroConditions)}.sql";

        public static string GetManualScoreRuleMacroConditions => $"{BaseDirectory}/{nameof(GetManualScoreRuleMacroConditions)}.sql";
    }
}