using System;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis
{
    public class SettingsKeyAnalyzers
    {
        private Terms ReportTerms { get; }

        public SettingsKeyAnalyzers(Terms reportTerms)
        {
            ReportTerms = reportTerms;
        }

        public CmsSettingsKeyResult CMSRegistrationEmailConfirmation(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSRegistrationEmailConfirmation);

        public CmsSettingsKeyResult CMSResetPasswordRequiresApproval(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSResetPasswordRequiresApproval);

        public CmsSettingsKeyResult CMSSendEmailWithResetPassword(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSSendEmailWithResetPassword);

        public CmsSettingsKeyResult CMSPasswordExpiration(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSPasswordExpiration);

        public CmsSettingsKeyResult CMSUsePasswordPolicy(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSUsePasswordPolicy);

        public CmsSettingsKeyResult CMSChatEnableFloodProtection(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSChatEnableFloodProtection);

        public CmsSettingsKeyResult CMSFloodProtectionEnabled(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSFloodProtectionEnabled);

        public CmsSettingsKeyResult CMSUseSSLForAdministrationInterface(CmsSettingsKey cmsSettingsKey)
            => UseStringAnalysis(cmsSettingsKey, ReportTerms.RecommendationReasons.CMSUseSSLForAdministrationInterface);

        private CmsSettingsKeyResult UseStringAnalysis(
            CmsSettingsKey cmsSettingsKey,
            Term recommendationReason,
            string recommendedValue = "true"
            )
        {
            var valueIsRecommended = cmsSettingsKey.KeyValue
                .Equals(recommendedValue, StringComparison.InvariantCultureIgnoreCase);

            if (valueIsRecommended) return null;

            return new CmsSettingsKeyResult(cmsSettingsKey, recommendedValue, recommendationReason);
        }
    }
}