using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Models.Results
{
    public class ReportResultsData : IEnumerable
    {
        protected IList<Result> results;

        internal ReportResultsData()
        {
            results = new List<Result>();
        }

        public static implicit operator ReportResultsData(Result result)
        {
            return new ReportResultsData
            {
                result
            };
        }

        public IEnumerator GetEnumerator() => results.GetEnumerator();

        public T First<T>() where T : Result
        {
            return results
                .OfType<T>()
                .First();
        }

        public bool Add(Result result, bool addIfNoData = false)
        {
            var added = addIfNoData || result.HasData;

            if (added)
            {
                results.Add(result);
            }

            return added;
        }
    }
}