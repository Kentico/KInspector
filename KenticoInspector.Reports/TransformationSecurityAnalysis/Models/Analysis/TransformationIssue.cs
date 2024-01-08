using System.Text.RegularExpressions;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis
{
    public class TransformationIssue
    {
        public static int SnippetPadding => 5;

        public static string SnippetWrapper => "...";

        public string CodeSnippet { get; }

        public string IssueType { get; }

        public TransformationIssue(string codeSnippet, string issueType)
        {
            CodeSnippet = codeSnippet;
            IssueType = issueType;
        }

        public static string ReplaceEachUppercaseLetterWithASpaceAndTheLetter(string issueType)
        {
            return Regex.Replace(issueType, "([A-Z])", " $1");
        }
    }
}