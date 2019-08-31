using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models.Results
{
    public class TaskCountResult
    {
        public Term Term { get; set; }

        public int Count { get; set; }

        public TaskCountResult(Term term, int count)
        {
            Term = term;
            Count = count;
        }
    }
}