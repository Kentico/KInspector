using System.Collections.Generic;

namespace KenticoInspector.Core.Models.Results
{
    public abstract class Result
    {
        public string Name { get; set; }

        public static implicit operator Result(string stringData) => new StringResult(stringData);

        public static implicit operator Result(Term term) => new StringResult(term);

        public static implicit operator string(Result result) => (result as StringResult)?.String;
    }

    public static class ResultExtensions
    {
        public static TableResult<T> AsResult<T>(this IEnumerable<T> rows, Term name)
        {
            return new TableResult<T>(rows, name);
        }
    }
}