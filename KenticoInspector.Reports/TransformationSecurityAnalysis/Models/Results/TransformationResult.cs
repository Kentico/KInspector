using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using KenticoInspector.Core.Helpers;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationResult : DynamicObject
    {
        private readonly IDictionary<string, string> dynamicIssueProperties = new Dictionary<string, string>();

        public const string snippetWrapper = "...";

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public int Uses { get; set; }

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
                    .Select(IssueSnippet)
                    .Aggregate(ResultsHelper.AggregateAsLines);

                dynamicIssueProperties[issueGroup.Key] = aggregatedSnippets;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var exists = dynamicIssueProperties.TryGetValue(binder.Name, out string value);

            result = value;

            return exists;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dynamicIssueProperties.Keys;
        }

        private string IssueSnippet(TransformationIssue issue)
        {
            return $"{snippetWrapper}{issue.CodeSnippet}{snippetWrapper}";
        }

        public static int TransformationUses(TransformationResult result)
        {
            return result.Uses;
        }
    }
}