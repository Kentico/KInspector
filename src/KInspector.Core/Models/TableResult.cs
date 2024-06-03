namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents a table containing results of a module's execution.
    /// </summary>
    public class TableResult
    {
        /// <summary>
        /// The name of the table.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The table rows.
        /// </summary>
        public IEnumerable<object> Rows { get; set; } = Enumerable.Empty<object>();
    }
}