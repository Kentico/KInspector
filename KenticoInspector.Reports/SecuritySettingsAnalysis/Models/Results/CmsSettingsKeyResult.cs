using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results
{
    public class CmsSettingsKeyResult
    {
        private readonly int siteID;
        private string categoryIDPath;

        public string SiteName { get; }

        public int KeyID { get; }

        public string KeyPath { get; }

        public string KeyDisplayName { get; }

        public string KeyDefaultValue { get; }

        public string KeyValue { get; }

        public string RecommendedValue { get; }

        public string RecommendationReason { get; }

        public CmsSettingsKeyResult(CmsSettingsKey cmsSettingsKey, string recommendedValue, Term recommendationReason)
        {
            siteID = cmsSettingsKey.SiteID;
            categoryIDPath = cmsSettingsKey.CategoryIDPath;

            KeyID = cmsSettingsKey.KeyID;
            KeyDisplayName = cmsSettingsKey.KeyDisplayName;
            KeyDefaultValue = cmsSettingsKey.KeyDefaultValue;
            KeyValue = cmsSettingsKey.KeyValue;
            RecommendedValue = recommendedValue;
            RecommendationReason = recommendationReason;
        }

        public CmsSettingsKeyResult(
            CmsSettingsKeyResult cmsSettingsKeyResult,
            IEnumerable<CmsSettingsCategory> cmsSettingsCategories,
            IEnumerable<Site> sites,
            IDictionary<string, string> resxValues
            )
        {
            SiteName = sites
                .FirstOrDefault(site => site.Id == cmsSettingsKeyResult.siteID)
                .Name;

            KeyID = cmsSettingsKeyResult.KeyID;

            var categoryDisplayNames = cmsSettingsKeyResult
                .GetCategoryIdsOnPath()
                .Select(idString => cmsSettingsCategories
                    .First(cmsSettingsCategory => cmsSettingsCategory
                        .CategoryID.ToString()
                        .Equals(idString))
                    .CategoryDisplayName)
                .Select(categoryDisplayName => TryReplaceDisplayName(resxValues, categoryDisplayName));

            KeyPath = string.Join(" > ", categoryDisplayNames);

            KeyDisplayName = TryReplaceDisplayName(
                resxValues,
                cmsSettingsKeyResult.KeyDisplayName
                );

            KeyDefaultValue = cmsSettingsKeyResult.KeyDefaultValue;
            KeyValue = cmsSettingsKeyResult.KeyValue;
            RecommendedValue = cmsSettingsKeyResult.RecommendedValue;
            RecommendationReason = cmsSettingsKeyResult.RecommendationReason;
        }

        public IEnumerable<string> GetCategoryIdsOnPath()
        {
            return categoryIDPath
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(pathSegment => pathSegment.TrimStart('0'));
        }

        private static string TryReplaceDisplayName(IDictionary<string, string> resxValues, string displayName)
        {
            displayName = displayName
                .Replace("{$", string.Empty)
                .Replace("$}", string.Empty)
                .ToLowerInvariant();

            if (resxValues.TryGetValue(displayName, out string keyName))
            {
                displayName = keyName;
            }

            return displayName;
        }
    }
}