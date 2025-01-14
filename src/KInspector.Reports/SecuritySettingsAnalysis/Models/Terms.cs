using KInspector.Core.Models;

namespace KInspector.Reports.SecuritySettingsAnalysis.Models
{
    public class Terms
    {
        public Term? GlobalSiteName { get; set; }

        public RecommendationReasons? RecommendationReasons { get; set; }

        public RecommendedValues? RecommendedValues { get; set; }

        public Summaries? Summaries { get; set; }

        public TableTitles? TableTitles { get; set; }
    }

    public class RecommendedValues
    {
        public Term? Empty { get; set; }

        public Term? InvalidLogonAttempts { get; set; }

        public Term? NoDangerousExtensions { get; set; }

        public Term? NotEmpty { get; set; }

        public Term? NotOn { get; set; }

        public Term? NotSaUser { get; set; }

        public Term? ResetPasswordInterval { get; set; }

        public Term? PasswordMinimalLength { get; set; }

        public Term? PasswordNumberOfNonAlphaNumChars { get; set; }

        public Term? ReCaptcha { get; set; }
    }

    public class RecommendationReasons
    {
        public AppSettings? AppSettings { get; set; }

        public ConnectionStrings? ConnectionStrings { get; set; }

        public SettingsKeys? SettingsKeys { get; set; }

        public SystemWebSettings? SystemWebSettings { get; set; }
    }

    public class AppSettings
    {
        public Term? CMSEnableCsrfProtection { get; set; }

        public Term? CMSHashStringSalt { get; set; }

        public Term? CMSRenewSessionAuthChange { get; set; }

        public Term? CMSXFrameOptionsExcluded { get; set; }
    }

    public class ConnectionStrings
    {
        public Term? SaUser { get; set; }
    }

    public class SettingsKeys
    {
        public Term? CMSAutocompleteEnableForLogin { get; set; }

        public Term? CMSCaptchaControl { get; set; }

        public Term? CMSChatEnableFloodProtection { get; set; }

        public Term? CMSFloodProtectionEnabled { get; set; }

        public Term? CMSForumAttachmentExtensions { get; set; }

        public Term? CMSMaximumInvalidLogonAttempts { get; set; }

        public Term? CMSMediaFileAllowedExtensions { get; set; }

        public Term? CMSPasswordExpiration { get; set; }

        public Term? CMSPasswordExpirationBehaviour { get; set; }

        public Term? CMSPasswordFormat { get; set; }

        public Term? CMSPolicyMinimalLength { get; set; }

        public Term? CMSPolicyNumberOfNonAlphaNumChars { get; set; }

        public Term? CMSRegistrationEmailConfirmation { get; set; }

        public Term? CMSResetPasswordInterval { get; set; }

        public Term? CMSRESTServiceEnabled { get; set; }

        public Term? CMSUploadExtensions { get; set; }

        public Term? CMSUsePasswordPolicy { get; set; }

        public Term? CMSUseSSLForAdministrationInterface { get; set; }
    }

    public class SystemWebSettings
    {
        public Term? AuthenticationCookieless { get; set; }

        public Term? CompilationDebug { get; set; }

        public Term? CustomErrorsMode { get; set; }

        public Term? HttpCookiesHttpOnlyCookies { get; set; }

        public Term? PagesEnableViewState { get; set; }

        public Term? PagesEnableViewStateMac { get; set; }

        public Term? TraceEnabled { get; set; }
    }

    public class Summaries
    {
        public Term? Warning { get; set; }

        public Term? Good { get; set; }
    }

    public class TableTitles
    {
        public Term? AdminSecuritySettings { get; set; }

        public Term? WebConfigSecuritySettings { get; set; }
    }
}