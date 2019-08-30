using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Models.Results
{
    public class TableResult<T> : Result
    {
        public IEnumerable<T> Rows { get; set; }

        public override bool HasData => Rows.Any();

        internal TableResult()
        {
        }

        internal TableResult(IEnumerable<T> rows)
        {
            Rows = rows;
        }

        public TableResult<T> WithLabel(Term tableLabel)
        {
            Label = tableLabel;

            return this;
        }
    }
}