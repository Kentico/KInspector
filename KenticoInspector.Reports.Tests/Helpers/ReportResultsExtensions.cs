using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.Tests.Helpers
{
    internal static class ReportResultsExtensions
    {
        public static TResult GetAnonymousTableResult<TResult>(this ReportResults results, string resultName)
        {
            if (results.Data is IDictionary<string, object> dictionary)
            {
                return dictionary[resultName] as dynamic;
            }

            return results
                .Data
                .GetType()
                .GetProperty(resultName)
                .GetValue(results.Data);
        }
    }
}