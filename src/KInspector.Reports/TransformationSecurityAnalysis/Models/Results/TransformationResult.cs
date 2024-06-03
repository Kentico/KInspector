using System.Dynamic;

using KInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;
using KInspector.Reports.TransformationSecurityAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationResult : DynamicObject
    {
        private readonly IDictionary<string, string?> dynamicIssueProperties = new Dictionary<string, string?>();

        [JsonProperty]
        public string Name { get; }

        [JsonProperty]
        public string Type { get; }

        [JsonProperty]
        public int Uses { get; }

        public TransformationResult(Transformation transformation, int uses, IEnumerable<string> detectedIssueTypes)
        {
            Name = transformation.FullName;
            Type = transformation.TransformationType.ToString();
            Uses = uses;

            foreach (var issueType in detectedIssueTypes)
            {
                dynamicIssueProperties.TryAdd(issueType, null);
            }

            var groupedIssues = transformation.Issues
                .GroupBy(issue => issue.IssueType);

            foreach (var issueGroup in groupedIssues)
            {
                var aggregatedSnippets = issueGroup
                    .Select(IssueSnippet);

                dynamicIssueProperties[issueGroup.Key] = string.Join(string.Empty, aggregatedSnippets);
            }
        }

        private string IssueSnippet(TransformationIssue issue)
        {
            return $"{TransformationIssue.SnippetWrapper}{issue.CodeSnippet}{TransformationIssue.SnippetWrapper}";
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            var exists = dynamicIssueProperties.TryGetValue(binder.Name, out string? value);

            result = value;

            return exists;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dynamicIssueProperties.Keys;
        }
    }
}