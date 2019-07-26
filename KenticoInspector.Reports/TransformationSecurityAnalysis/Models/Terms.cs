using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models
{
    public class Terms
    {
        public Term NoIssues { get; set; }

        public Term Issues { get; set; }

        public Term IssueTypes { get; set; }

        public Term TransformationsWithIssues { get; set; }

        public Term TransformationUsage { get; set; }

        public Term TemplateUsage { get; set; }

        public Term XssQueryHelper { get; set; }

        public Term XssQueryString { get; set; }

        public Term XssHttpContext { get; set; }

        public Term XssServer { get; set; }

        public Term XssRequest { get; set; }

        public Term XssDocument { get; set; }

        public Term XssWindow { get; set; }

        public Term ServerSideScript { get; set; }

        public Term DocumentsMacro { get; set; }

        public Term QueryMacro { get; set; }
    }
}