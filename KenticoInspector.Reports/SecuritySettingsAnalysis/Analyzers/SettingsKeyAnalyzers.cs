using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class SettingsKeyAnalyzers : AbstractAnalyzers<CmsSettingsKey, CmsSettingsKeyResult>
    {
        private readonly IEnumerable<string> dangerousExtensions = new[] { "exe", "src", "cs", "dll", "aspx", "ascx" };

        public SettingsKeyAnalyzers(Terms reportTerms) : base(reportTerms)
        {
        }

        public CmsSettingsKeyResult CMSAutocompleteEnableForLogin(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "false",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSAutocompleteEnableForLogin
                );

        public CmsSettingsKeyResult CMSCaptchaControl(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => value.Equals("3", StringComparison.InvariantCultureIgnoreCase),
                ReportTerms.RecommendedValues.ReCaptcha,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSCaptchaControl
                );

        public CmsSettingsKeyResult CMSChatEnableFloodProtection(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSChatEnableFloodProtection
                );

        public CmsSettingsKeyResult CMSFloodProtectionEnabled(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSFloodProtectionEnabled
                );

        public CmsSettingsKeyResult CMSForumAttachmentExtensions(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => !value.Split(';')
                    .Any(ext => dangerousExtensions.Contains(ext.ToLower())),
                ReportTerms.RecommendedValues.NoDangerousExtensions,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSForumAttachmentExtensions
                );

        public CmsSettingsKeyResult CMSMaximumInvalidLogonAttempts(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => int.Parse(value) <= 5,
                ReportTerms.RecommendedValues.InvalidLogonAttempts,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSMaximumInvalidLogonAttempts
                );

        public CmsSettingsKeyResult CMSMediaFileAllowedExtensions(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => !value.Split(';')
                    .Any(ext => dangerousExtensions.Contains(ext.ToLower())),
                ReportTerms.RecommendedValues.NoDangerousExtensions,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSMediaFileAllowedExtensions
                );

        public CmsSettingsKeyResult CMSPasswordExpiration(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPasswordExpiration
                );

        public CmsSettingsKeyResult CMSPasswordFormat(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "PBKDF2",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPasswordFormat
                );

        public CmsSettingsKeyResult CMSPolicyMinimalLength(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => int.Parse(value) >= 8,
                ReportTerms.RecommendedValues.PasswordMinimalLength,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPolicyMinimalLength
                );

        public CmsSettingsKeyResult CMSPolicyNumberOfNonAlphaNumChars(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => int.Parse(value) >= 2,
                ReportTerms.RecommendedValues.PasswordNumberOfNonAlphaNumChars,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPolicyNumberOfNonAlphaNumChars
                );

        public CmsSettingsKeyResult CMSRegistrationEmailConfirmation(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSRegistrationEmailConfirmation
                );

        public CmsSettingsKeyResult CMSResetPasswordInterval(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => int.Parse(value) >= 1 && int.Parse(value) <= 12,
                ReportTerms.RecommendedValues.ResetPasswordInterval,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSResetPasswordInterval
                );

        public CmsSettingsKeyResult CMSResetPasswordRequiresApproval(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSResetPasswordRequiresApproval
                );

        public CmsSettingsKeyResult CMSRESTServiceEnabled(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSRESTServiceEnabled
                );

        public CmsSettingsKeyResult CMSSendEmailWithResetPassword(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSSendEmailWithResetPassword
                );

        public CmsSettingsKeyResult CMSUploadExtensions(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingFunc(
                cmsSettingsKey,
                value => !value.Split(';')
                    .Any(ext => dangerousExtensions.Contains(ext.ToLower())),
                ReportTerms.RecommendedValues.NoDangerousExtensions,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSUploadExtensions
                );

        public CmsSettingsKeyResult CMSUsePasswordPolicy(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSUsePasswordPolicy
                );

        public CmsSettingsKeyResult CMSUseSSLForAdministrationInterface(CmsSettingsKey cmsSettingsKey)
            => AnalyzeUsingString(
                cmsSettingsKey,
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSUseSSLForAdministrationInterface
                );

        protected override CmsSettingsKeyResult AnalyzeUsingFunc(
            CmsSettingsKey cmsSettingsKey,
            Func<string, bool> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            if (valueIsRecommended(cmsSettingsKey.KeyValue)) return null;

            return new CmsSettingsKeyResult(cmsSettingsKey, recommendedValue, recommendationReason);
        }
    }
}