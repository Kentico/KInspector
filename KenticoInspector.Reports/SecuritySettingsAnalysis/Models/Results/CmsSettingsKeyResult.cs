using KenticoInspector.Core.Models;
using Newtonsoft.Json;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results
{
    public class CmsSettingsKeyResult
    {
        public string SiteName { get; set; }

        [JsonIgnore]
        public int SiteID { get; }

        public int KeyID { get; }

        [JsonIgnore]
        public string CategoryIDPath { get; }

        public string KeyPath { get; set; }

        public string KeyDisplayName { get; set; }

        public string KeyDefaultValue { get; }

        public string KeyValue { get; }

        public string RecommendedValue { get; }

        public string RecommendationReason { get; }

        public CmsSettingsKeyResult(CmsSettingsKey cmsSettingsKey, string recommendedValue, Term recommendationReason)
        {
            SiteID = cmsSettingsKey.SiteID;
            KeyID = cmsSettingsKey.KeyID;
            CategoryIDPath = cmsSettingsKey.CategoryIDPath;

            KeyDisplayName = cmsSettingsKey.KeyDisplayName;
            KeyDefaultValue = cmsSettingsKey.KeyDefaultValue;
            KeyValue = cmsSettingsKey.KeyValue;
            RecommendedValue = recommendedValue;
            RecommendationReason = recommendationReason;
        }
    }
}