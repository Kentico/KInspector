using System;
using System.Text.RegularExpressions;

namespace KenticoInspector.Core.Helpers
{
    public class ResultsHelper
    {
        private static readonly string newLine = Environment.NewLine;

        public static string AggregateAsLines(object left, object right)
        {
            return $"{left}{newLine}{right}";
        }

        public static string ToTitleCase(string issueType)
        {
            return Regex.Replace(issueType, "([A-Z])", " $1");
        }
    }
}