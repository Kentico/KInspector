using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models
{
    public class Terms
    {
        public Term GlobalSiteName { get; set; }

        public RecommendationReasons RecommendationReasons { get; set; }

        public RecommendedValues RecommendedValues { get; set; }

        public Summaries Summaries { get; set; }

        public TableTitles TableTitles { get; set; }
    }

    public class RecommendedValues
    {
        public Term NotEmpty { get; set; }
    }

    public class RecommendationReasons
    {
        public Term CMSRegistrationEmailConfirmation { get; set; }

        public Term CMSResetPasswordRequiresApproval { get; set; }

        public Term CMSSendEmailWithResetPassword { get; set; }

        public Term CMSPasswordExpiration { get; set; }

        public Term CMSUsePasswordPolicy { get; set; }

        public Term CMSChatEnableFloodProtection { get; set; }

        public Term CMSFloodProtectionEnabled { get; set; }

        public Term CMSUseSSLForAdministrationInterface { get; set; }

        public Term CMSRenewSessionAuthChange { get; set; }

        public Term CMSHashStringSalt { get; set; }

        public Term CompilationDebug { get; set; }

        public Term TraceEnabled { get; set; }
    }

    public class Summaries
    {
        public Term Good { get; set; }

        public Term Error { get; set; }
    }

    public class TableTitles
    {
        public Term AdminSecuritySettings { get; set; }

        public Term WebConfigSecuritySettings { get; set; }
    }
}