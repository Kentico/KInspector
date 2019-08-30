using System.Collections.Generic;

namespace KenticoInspector.Core.Models.Results
{
    public class TableResult<T> : Result
    {
        public IEnumerable<T> Rows { get; set; }

        internal TableResult()
        {

        }

        internal TableResult(IEnumerable<T> rows, Term name)
        {
            Rows = rows;
            Name = name;
        }
    }
}