﻿using System.Linq.Expressions;

using KInspector.Core.Models;
using KInspector.Reports.SecuritySettingsAnalysis.Models;
using KInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KInspector.Reports.SecuritySettingsAnalysis.Models.Results;

namespace KInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class SettingsKeyAnalyzers : AbstractAnalyzers<CmsSettingsKey, CmsSettingsKeyResult?>
    {
        private readonly IEnumerable<string> dangerousExtensions = new[] { "exe", "src", "cs", "dll", "aspx", "ascx", "msi", "bat" };

        public override IEnumerable<Expression<Func<CmsSettingsKey, CmsSettingsKeyResult?>>> Analyzers
            => new List<Expression<Func<CmsSettingsKey, CmsSettingsKeyResult?>>>
        {
            CMSAutocompleteEnableForLogin => AnalyzeUsingExpression(
                CMSAutocompleteEnableForLogin,
                value => Equals(value, "false"),
                "false",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSAutocompleteEnableForLogin
                ),
            CMSCaptchaControl => AnalyzeUsingExpression(
                CMSCaptchaControl,
                value => Equals(value, "3"),
                ReportTerms.RecommendedValues.ReCaptcha,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSCaptchaControl
                ),
            CMSChatEnableFloodProtection => AnalyzeUsingExpression(
                CMSChatEnableFloodProtection,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSChatEnableFloodProtection
                ),
            CMSFloodProtectionEnabled => AnalyzeUsingExpression(
                CMSFloodProtectionEnabled,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSFloodProtectionEnabled
                ),
            CMSForumAttachmentExtensions => AnalyzeUsingExpression(
                CMSForumAttachmentExtensions,
                value => !value.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Any(ext => dangerousExtensions.Contains(ext.ToLower())),
                ReportTerms.RecommendedValues.NoDangerousExtensions,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSForumAttachmentExtensions
                ),
            CMSMaximumInvalidLogonAttempts => AnalyzeUsingExpression(
                CMSMaximumInvalidLogonAttempts,
                value => int.Parse(value) <= 5,
                ReportTerms.RecommendedValues.InvalidLogonAttempts,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSMaximumInvalidLogonAttempts
                ),
            CMSMediaFileAllowedExtensions => AnalyzeUsingExpression(
                CMSMediaFileAllowedExtensions,
                value => !value.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Any(ext => dangerousExtensions.Contains(ext.ToLower())),
                ReportTerms.RecommendedValues.NoDangerousExtensions,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSMediaFileAllowedExtensions
                ),
            CMSPasswordExpiration => AnalyzeUsingExpression(
                CMSPasswordExpiration,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPasswordExpiration
                ),
            CMSPasswordExpirationBehaviour => AnalyzeUsingExpression(
                CMSPasswordExpirationBehaviour,
                value => Equals(value, "LOCKACCOUNT"),
                "LOCKACCOUNT",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPasswordExpirationBehaviour
                ),
            CMSPasswordFormat => AnalyzeUsingExpression(
                CMSPasswordFormat,
                value => Equals(value, "PBKDF2"),
                "PBKDF2",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPasswordFormat
                ),
            CMSPolicyMinimalLength => AnalyzeUsingExpression(
                CMSPolicyMinimalLength,
                value => int.Parse(value) >= 8,
                ReportTerms.RecommendedValues.PasswordMinimalLength,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPolicyMinimalLength
                ),
            CMSPolicyNumberOfNonAlphaNumChars => AnalyzeUsingExpression(
                CMSPolicyNumberOfNonAlphaNumChars,
                value => int.Parse(value) >= 2,
                ReportTerms.RecommendedValues.PasswordNumberOfNonAlphaNumChars,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSPolicyNumberOfNonAlphaNumChars
                ),
            CMSRegistrationEmailConfirmation => AnalyzeUsingExpression(
                CMSRegistrationEmailConfirmation,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSRegistrationEmailConfirmation
                ),
            CMSResetPasswordInterval => AnalyzeUsingExpression(
               CMSResetPasswordInterval,
                value => int.Parse(value) >= 1 && int.Parse(value) <= 12,
                ReportTerms.RecommendedValues.ResetPasswordInterval,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSResetPasswordInterval
                ),
            CMSRESTServiceEnabled => AnalyzeUsingExpression(
                CMSRESTServiceEnabled,
                value => Equals(value, "false"),
                "false",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSRESTServiceEnabled
                ),
            CMSUploadExtensions => AnalyzeUsingExpression(
                CMSUploadExtensions,
                value => !value.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Any(ext => dangerousExtensions.Contains(ext.ToLower())),
                ReportTerms.RecommendedValues.NoDangerousExtensions,
                ReportTerms.RecommendationReasons.SettingsKeys.CMSUploadExtensions
                ),
            CMSUsePasswordPolicy => AnalyzeUsingExpression(
                CMSUsePasswordPolicy,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSUsePasswordPolicy
                ),
            CMSUseSSLForAdministrationInterface => AnalyzeUsingExpression(
                CMSUseSSLForAdministrationInterface,
                value => Equals(value, "true"),
                "true",
                ReportTerms.RecommendationReasons.SettingsKeys.CMSUseSSLForAdministrationInterface
                )
        };

        public SettingsKeyAnalyzers(Terms? reportTerms) : base(reportTerms)
        {
        }

        protected override CmsSettingsKeyResult? AnalyzeUsingExpression(
            CmsSettingsKey cmsSettingsKey,
            Expression<Func<string, bool>> valueIsRecommended,
            string? recommendedValue,
            Term? recommendationReason
            )
        {
            string? keyValue = cmsSettingsKey.KeyValue;
            if (keyValue is null || recommendedValue is null || recommendationReason is null || valueIsRecommended.Compile()(keyValue))
            {
                return null;
            }

            return new CmsSettingsKeyResult(cmsSettingsKey, recommendedValue, recommendationReason);
        }
    }
}