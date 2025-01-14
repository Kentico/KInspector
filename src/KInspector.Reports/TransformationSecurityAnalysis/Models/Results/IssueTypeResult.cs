using KInspector.Core.Models;
using KInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;

namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class IssueTypeResult
    {
        public string Name { get; }

        public string Description { get; }

        public IssueTypeResult(string issueType, IDictionary<string, Term?> detectedIssueTypes)
        {
            detectedIssueTypes.TryGetValue(issueType, out Term? description);

            Name = TransformationIssue.ReplaceEachUppercaseLetterWithASpaceAndTheLetter(issueType);
            Description = description;
        }
    }
}