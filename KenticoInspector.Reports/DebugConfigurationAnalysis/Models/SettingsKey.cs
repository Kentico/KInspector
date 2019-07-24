namespace KenticoInspector.Reports.DebugConfigurationAnalysis.Models
{
    public class SettingsKey
    {
        public string KeyName { get; set; }
        public string KeyDisplayName { get; set; }
        public bool KeyValue { get; set; }
        public bool KeyDefaultValue { get; set; }

        public SettingsKey() { }

        public SettingsKey(string keyName, string keyDisplayName, bool keyValue, bool keyDefaultValue)
        {
            KeyName = keyName;
            KeyDisplayName = keyDisplayName;
            KeyValue = keyValue;
            KeyDefaultValue = keyDefaultValue;
        }
    }
}
