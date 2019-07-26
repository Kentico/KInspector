using System.Collections.Generic;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class IssueTypeResult
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IssueTypeResult(string issueType, IDictionary<string, Term> detectedIssueTypes)
        {
            detectedIssueTypes.TryGetValue(issueType, out Term description);

            Name = ResultsHelper.ToTitleCase(issueType);
            Description = description;
        }
    }
}