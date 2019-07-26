using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    [DebuggerDisplay("{FullName} Issues:{Issues.Count}")]
    public class Transformation
    {
        public string FullName { get; }

        public TransformationType TransformationType { get; }

        public string Code { get; }

        public IList<TransformationIssue> Issues { get; }

        public Transformation(TransformationDto transformationDto)
        {
            FullName = $"{transformationDto.ClassName}.{transformationDto.TransformationName}";
            TransformationType = transformationDto.Type;
            Code = transformationDto.TransformationCode;

            Issues = new List<TransformationIssue>();
        }

        public static IEnumerable<TransformationIssue> TransformationIssues(Transformation transformation)
        {
            return transformation.Issues;
        }

        public static bool TransformationHasIssues(Transformation transformation)
        {
            return TransformationIssues(transformation)
                .Any();
        }

        public static string TransformationFullName(Transformation transformation)
        {
            return transformation.FullName;
        }

        public void AddIssue(int snippetStartIndex, int snippetLength, string issueType)
        {
            var startIndex = Math.Max(snippetStartIndex - TransformationIssue.SnippetPadding, 0);
            var length = Math.Min(Code.Length - startIndex, snippetLength + TransformationIssue.SnippetPadding * 2);

            Issues.Add(
                new TransformationIssue(
                        Code.Substring(startIndex, length),
                        issueType
                    )
                );
        }
    }
}