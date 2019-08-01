using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis
{
    /// <summary>
    /// Contains instance methods returning <see cref="void"/> that are called by the report to analyze a single <see cref="Transformation"/>.
    /// Each method adds issues using <see cref="Transformation.AddIssue(int, int, string)"/>. If there are any issues found, it also adds itself to <see cref="DetectedIssueTypes"/>.
    /// </summary>
    public class IssueAnalyzers
    {
        private Terms ReportTerms { get; }

        public static IDictionary<string, Term> DetectedIssueTypes { get; set; } = new Dictionary<string, Term>();

        public IssueAnalyzers(Terms reportTerms)
        {
            ReportTerms = reportTerms;
        }

        public void XssQueryHelper(Transformation transformation) => UseRegexAnalysis(transformation, "queryhelper\\.", ReportTerms.IssueDescriptions.XssQueryHelper);

        public void XssQueryString(Transformation transformation) => UseRegexAnalysis(transformation, "[ (.]querystring", ReportTerms.IssueDescriptions.XssQueryString);

        public void XssHttpContext(Transformation transformation) => UseRegexAnalysis(transformation, "[ (.]httpcontext\\.", ReportTerms.IssueDescriptions.XssHttpContext);

        public void XssServer(Transformation transformation) => UseRegexAnalysis(transformation, "[ (.]server\\.", ReportTerms.IssueDescriptions.XssServer);

        public void XssRequest(Transformation transformation) => UseRegexAnalysis(transformation, "[ (.]request\\.", ReportTerms.IssueDescriptions.XssRequest);

        public void XssDocument(Transformation transformation) => UseRegexAnalysis(transformation, "<script .*?document\\.", ReportTerms.IssueDescriptions.XssDocument);

        public void XssWindow(Transformation transformation) => UseRegexAnalysis(transformation, "window\\.", ReportTerms.IssueDescriptions.XssWindow);

        public void ServerSideScript(Transformation transformation) => UseRegexAnalysis(transformation, "<script runat=\"?server\"?", ReportTerms.IssueDescriptions.ServerSideScript);

        public void DocumentsMacro(Transformation transformation) => UseRegexAnalysis(transformation, "{%.*?documents[[.]", ReportTerms.IssueDescriptions.DocumentsMacro);

        public void QueryMacro(Transformation transformation) => UseRegexAnalysis(transformation, "{\\?.*|{%.*querystring", ReportTerms.IssueDescriptions.QueryMacro);

        private void UseRegexAnalysis(Transformation transformation, string pattern, Term issueDescription, [CallerMemberName]string issueType = null)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var regexMatches = regex.Matches(transformation.Code);

            if (regexMatches.Count == 0) return;

            DetectedIssueTypes.TryAdd(issueType, issueDescription);

            foreach (Match match in regex.Matches(transformation.Code))
            {
                transformation.AddIssue(match.Index, match.Length, issueType);
            }
        }
    }
}