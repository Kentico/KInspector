using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Models.Results
{
    public class ReportResultsData : ReportResultsData<Result>
    {
        internal ReportResultsData()
        {
        }

        private ReportResultsData(List<Result> listResults)
        {
            results = listResults;
        }

        public static implicit operator ReportResultsData(List<Result> listResults)
        {
            return new ReportResultsData(listResults);
        }

        public static implicit operator ReportResultsData(Result result)
        {
            return new ReportResultsData(
                new List<Result>
                {
                    result
                }
            );
        }

        public T First<T>() where T : Result
        {
            return this
                .OfType<T>()
                .First();
        }

        public int AddIfAny<T>(TableResult<T> tableResult)
        {
            if (tableResult.Rows.Any())
            {
                Add(tableResult);
            }

            return results.Count();
        }
    }

    public class ReportResultsData<T> : IList<T> where T : Result
    {
        protected List<T> results;

        public int Count => results.Count;

        public bool IsReadOnly => false;

        public T this[int index] { get => results[index]; set => results[index] = value; }

        internal ReportResultsData()
        {
            results = new List<T>();
        }

        public void Add(T item) => results.Add(item);

        public void Clear() => results.Clear();

        public bool Contains(T item) => results.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => results.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => results.GetEnumerator();

        public int IndexOf(T item) => results.IndexOf(item);

        public void Insert(int index, T item) => results.Insert(index, item);

        public bool Remove(T item) => results.Remove(item);

        public void RemoveAt(int index) => results.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => results.GetEnumerator();
    }
}