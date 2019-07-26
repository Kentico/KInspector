namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis
{
    public class TransformationIssue
    {
        public const int SnippetPadding = 5;

        public string CodeSnippet { get; set; }

        public string IssueType { get; set; }

        public TransformationIssue(string codeSnippet, string issueType)
        {
            CodeSnippet = codeSnippet;
            IssueType = issueType;
        }

        public static string IssueIssueType(TransformationIssue transformationIssue)
        {
            return transformationIssue.IssueType;
        }
    }
}