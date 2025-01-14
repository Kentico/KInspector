namespace KInspector.Reports.SecuritySettingsAnalysis.Models.Data
{
    public class CmsSettingsKey
    {
        public int SiteID { get; set; }

        public int KeyID { get; set; }

        public string? KeyName { get; set; }

        public string? KeyDisplayName { get; set; }

        public string? KeyValue { get; set; }

        public string? KeyDefaultValue { get; set; }

        public int KeyCategoryID { get; set; }

        public string? CategoryIDPath { get; set; }
    }
}