using KInspector.Core.Constants;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents the results of a module's execution.
    /// </summary>
    public class ModuleResults
    {
        /// <summary>
        /// Indicates whether the module has been executed and its status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsStatus Status { get; set; } = ResultsStatus.NotRun;

        /// <summary>
        /// The short description of the execution's results.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Indicates whether the execution returned results, and of what type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsType Type { get; set; } = ResultsType.NoResults;

        /// <summary>
        /// The list of tables returned by the execution, if any.
        /// </summary>
        public IList<TableResult> TableResults { get; } = new List<TableResult>();

        /// <summary>
        /// The list of strings returned by the execution, if any.
        /// </summary>
        public IList<string> StringResults { get; } = new List<string>();
    }
}