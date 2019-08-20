using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results
{
    public class WebConfigSettingResult
    {
        public string KeyPath { get; set; }

        public string KeyName { get; set; }

        public string KeyValue { get; set; }

        public string RecommendedValue { get; set; }

        public string RecommendationReason { get; set; }

        public WebConfigSettingResult(WebConfigSetting webConfigSetting, string recommendedValue, Term recommendationReason)
        {
            KeyPath = webConfigSetting.KeyPath;
            KeyName = webConfigSetting.KeyName;
            KeyValue = webConfigSetting.KeyValue;
            RecommendedValue = recommendedValue;
            RecommendationReason = recommendationReason;
        }
    }
}