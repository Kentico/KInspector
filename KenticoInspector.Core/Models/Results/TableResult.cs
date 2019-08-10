using System.Collections.Generic;

namespace KenticoInspector.Core.Models.Results
{
    public class TableResult<T> : Result
    {
        public IEnumerable<T> Rows { get; set; }
    }
}